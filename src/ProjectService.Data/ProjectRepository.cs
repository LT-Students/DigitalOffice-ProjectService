using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class ProjectRepository : IProjectRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public DbProject Get(GetProjectFilter filter)
    {
      IQueryable<DbProject> dbProjectQueryable = _provider.Projects.AsQueryable();

      if (filter.IncludeUsers.HasValue && filter.IncludeUsers.Value)
      {
        if (filter.ShowNotActiveUsers.HasValue && filter.ShowNotActiveUsers.Value)
        {
          dbProjectQueryable = dbProjectQueryable.Include(x => x.Users);
        }
        else
        {
          dbProjectQueryable = dbProjectQueryable.Include(x => x.Users.Where(x => x.IsActive));
        }
      }

      if (filter.IncludeFiles.HasValue && filter.IncludeFiles.Value)
      {
        dbProjectQueryable = dbProjectQueryable.Include(x => x.Files);
      }

      if (filter.IncludeImages.HasValue && filter.IncludeImages.Value)
      {
        dbProjectQueryable = dbProjectQueryable.Include(x => x.Images);
      }

      DbProject dbProject = dbProjectQueryable.FirstOrDefault(x => x.Id == filter.ProjectId);

      if (dbProject == null)
      {
        return null;
      }

      return dbProject;
    }

    public IEnumerable<DbProject> Get(Guid departmentId)
    {
      return _provider.Projects.Where(p => p.DepartmentId == departmentId);
    }

    public Guid Create(DbProject dbProject)
    {
      _provider.Projects.Add(dbProject);
      _provider.Save();

      return dbProject.Id;
    }

    public bool Edit(Guid projectId, JsonPatchDocument<DbProject> request)
    {
      DbProject dbProject = Get(new GetProjectFilter { ProjectId = projectId });
      request.ApplyTo(dbProject);
      dbProject.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbProject.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();

      return true;
    }

    public void DisableWorkersInProject(Guid projectId, IEnumerable<Guid> userIds)
    {
      DbProject dbProject = _provider.Projects
        .FirstOrDefault(p => p.Id == projectId);

      if (dbProject == null)
      {
        throw new NotFoundException($"Project with Id {projectId} does not exist.");
      }

      foreach (Guid userId in userIds)
      {
        DbProjectUser dbProjectUser = dbProject.Users?.FirstOrDefault(w => w.UserId == userId);

        if (dbProjectUser == null)
        {
          throw new NotFoundException($"Worker with Id {userId} does not exist.");
        }

        dbProjectUser.IsActive = false;
      }

      _provider.Projects.Update(dbProject);
      _provider.Save();
    }

    public List<DbProject> Find(FindProjectsFilter filter, out int totalCount)
    {
      if (filter == null)
      {
        totalCount = 0;
        return null;
      }

      IQueryable<DbProject> dbProjects = _provider.Projects
        .AsQueryable();

      if (filter.DepartmentId.HasValue)
      {
        dbProjects = dbProjects.Where(p => p.DepartmentId == filter.DepartmentId.Value);
      }

      totalCount = dbProjects.Count();

      return dbProjects.Skip(filter.SkipCount).Take(filter.TakeCount).ToList();
    }

    public List<DbProject> Search(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return null;
      }

      return _provider.Projects.Where(p => p.Name.Contains(text) || p.ShortName.Contains(text)).ToList();
    }

    public bool IsExist(Guid id)
    {
      return _provider.Projects.FirstOrDefault(x => x.Id == id) != null;
    }

    public bool DoesProjectNameExist(string name)
    {
      return _provider.Projects.Any(p => p.Name.Contains(name));
    }

    public List<DbProject> Find(List<Guid> projectIds)
    {
      return _provider.Projects.Where(p => projectIds.Contains(p.Id)).ToList();
    }

    public List<DbProject> Get(IGetProjectsRequest request, out int totalCount)
    {
      IQueryable<DbProject> projects = _provider.Projects.AsQueryable();

      if (request.UserId.HasValue)
      {
        if (request.IncludeUsers)
        {
          projects = _provider.Projects
            .Include(pu => pu.Users)
            .Where(p => p.Users.Any(u => u.UserId == request.UserId.Value));
        }
        else
        {
          projects = _provider.ProjectsUsers
            .Where(pu => pu.UserId == request.UserId)
            .Include(pu => pu.Project)
            .Select(pu => pu.Project);
        }
      }

      if (request.ProjectsIds != null && request.ProjectsIds.Any())
      {
        projects = projects.Where(p => request.ProjectsIds.Contains(p.Id));
      }

      if (request.DepartmentId.HasValue)
      {
        projects = projects.Where(p => p.DepartmentId == request.DepartmentId.Value);
      }

      totalCount = projects.Count();

      if (request.SkipCount.HasValue)
      {
        projects = projects.Skip(request.SkipCount.Value);
      }

      if (request.TakeCount.HasValue)
      {
        projects = projects.Take(request.TakeCount.Value);
      }

      if (request.IncludeUsers && !request.UserId.HasValue)
      {
        projects = projects.Include(p => p.Users);
      }

      return projects.ToList();
    }
  }
}
