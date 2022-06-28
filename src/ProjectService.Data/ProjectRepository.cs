using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    #region private

    private IQueryable<DbProject> CreateGetPredicate(
      GetProjectFilter filter)
    {
      IQueryable<DbProject> projectsQuery = _provider.Projects.AsQueryable();

      if (filter.IncludeUsers)
      {
        projectsQuery = filter.ShowNotActiveUsers
          ? projectsQuery.Include(x => x.Users)
          : projectsQuery.Include(x => x.Users.Where(x => x.IsActive));
      }

      if (filter.IncludeFiles)
      {
        projectsQuery = projectsQuery.Include(x => x.Files);
      }

      if (filter.IncludeImages)
      {
        projectsQuery = projectsQuery.Include(x => x.Images);
      }

      return projectsQuery;
    }

    private IQueryable<DbProject> CreateGetPredicate(
      IGetProjectsRequest request)
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

      return projects;
    }

    #endregion

    public ProjectRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(DbProject dbProject, int usersCount)> GetAsync(GetProjectFilter filter)
    {
      return (
        await CreateGetPredicate(filter).FirstOrDefaultAsync(p => p.Id == filter.ProjectId),
        await _provider.ProjectsUsers.CountAsync(pu => pu.ProjectId == filter.ProjectId && pu.IsActive));
    }

    public async Task<(List<DbProject>, int totalCount)> GetAsync(IGetProjectsRequest request)
    {
      IQueryable<DbProject> projects = CreateGetPredicate(request);

      int totalCount = await projects.CountAsync();

      if (request.AscendingSort.HasValue)
      {
        projects = request.AscendingSort.Value
          ? projects.OrderBy(p => p.Name)
          : projects.OrderByDescending(p => p.Name);
      }

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

      return (await projects.ToListAsync(), totalCount);
    }

    public async Task<Guid?> CreateAsync(DbProject dbProject)
    {
      if (dbProject == null)
      {
        return null;
      }

      _provider.Projects.Add(dbProject);
      await _provider.SaveAsync();

      return dbProject.Id;
    }

    public async Task<bool> EditAsync(Guid projectId, JsonPatchDocument<DbProject> request)
    {
      DbProject dbProject = await _provider.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

      if (dbProject == null)
      {
        return false;
      }

      request.ApplyTo(dbProject);
      dbProject.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbProject.ModifiedAtUtc = DateTime.UtcNow;

      await _provider.SaveAsync();

      return true;
    }

    public async Task<(List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount)> FindAsync(FindProjectsFilter filter)
    {
      if (filter == null)
      {
        return (null, 0);
      }

      IQueryable<DbProject> dbProjectsQuery = _provider.Projects
        .AsQueryable();

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        dbProjectsQuery = dbProjectsQuery
          .Where(p =>
            p.Name.ToUpper().Contains(filter.NameIncludeSubstring.ToUpper())
            || p.ShortName.ToUpper().Contains(filter.NameIncludeSubstring.ToUpper()));
      }

      if (filter.ProjectStatus.HasValue)
      {
        dbProjectsQuery = dbProjectsQuery
          .Where(p => p.Status == (int)filter.ProjectStatus);
      }

      if (filter.IsAscendingSort.HasValue)
      {
        dbProjectsQuery = filter.IsAscendingSort.Value
          ? dbProjectsQuery.OrderBy(p => p.Name)
          : dbProjectsQuery.OrderByDescending(p => p.Name);
      }

      List<(DbProject dbProject, int usersCount)> dbProjects =
        (await
          (from project in dbProjectsQuery.Skip(filter.SkipCount).Take(filter.TakeCount)
           select new
           {
             Project = project,
             UsersCount = _provider.ProjectsUsers.Count(pu => pu.ProjectId == project.Id && pu.IsActive)
           }).ToListAsync())
           .Select(p => (p.Project, p.UsersCount)).ToList();

      int totalCount = dbProjects.Count();

      return (dbProjects, totalCount);
    }

    public async Task<List<DbProject>> SearchAsync(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return null;
      }

      return await _provider.Projects.Where(p => p.Name.Contains(text) || p.ShortName.Contains(text)).ToListAsync();
    }

    public async Task<bool> DoesExistAsync(Guid projectId)
    {
      return await _provider.Projects.AnyAsync(x => x.Id == projectId);
    }

    public async Task<bool> DoesNameExistAsync(string name)
    {
      return await _provider.Projects.AnyAsync(p => p.Name.ToLower().Equals(name.ToLower()));
    }

    public async Task<bool> DoesShortNameExistAsync(string shortName)
    {
      return await _provider.Projects.AnyAsync(p => p.ShortName.ToLower().Equals(shortName.ToLower()));
    }

    public async Task<List<Guid>> DoExistAsync(List<Guid> projectsIds)
    {
      return await _provider.Projects.Where(p => projectsIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
    }
  }
}
