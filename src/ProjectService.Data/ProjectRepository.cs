﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
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
      IQueryable<DbProject> query = _provider.Projects.AsQueryable();

      if (filter.IncludeProjectUsers)
      {
        query = query.Include(x => x.Users.Where(pu => pu.IsActive));
      }

      if (filter.IncludeDepartment && query.Any(p => p.Id == filter.ProjectId && p.Department.IsActive))
      {
        query = query.Include(x => x.Department);
      }

      return query;
    }

    private IQueryable<DbProject> CreateGetPredicate(
      IGetProjectsRequest request)
    {
      IQueryable<DbProject> projectsQuery = _provider.Projects.AsQueryable();

      if (request.ProjectsIds is not null && request.ProjectsIds.Any())
      {
        projectsQuery = projectsQuery.Where(p => request.ProjectsIds.Contains(p.Id));
      }

      if (request.UsersIds is not null && request.UsersIds.Any())
      {
        projectsQuery = projectsQuery.Where(p => p.Users.Any(u => request.UsersIds.Contains(u.UserId)));
      }      

      if (request.DepartmentsIds is not null && request.DepartmentsIds.Any())
      {
        projectsQuery = projectsQuery.Where(p => p.Department != null && request.DepartmentsIds.Contains(p.Department.DepartmentId));
      }

      if (request.IncludeUsers)
      {
        projectsQuery = projectsQuery.Include(pu => pu.Users);
      }

      if (request.IncludeDepartment)
      {
        projectsQuery = projectsQuery.Include(p => p.Department);
      }

      return projectsQuery;
    }

    private IQueryable<DbProject> CreateFindPredicate(FindProjectsFilter filter)
    {
      IQueryable<DbProject> query = _provider.Projects.AsQueryable();

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        query = query
          .Where(p =>
            p.Name.ToUpper().Contains(filter.NameIncludeSubstring.ToUpper())
            || p.ShortName.ToUpper().Contains(filter.NameIncludeSubstring.ToUpper()));
      }

      if (filter.IsAscendingSort.HasValue)
      {
        query = filter.IsAscendingSort.Value
          ? query.OrderBy(p => p.Name)
          : query.OrderByDescending(p => p.Name);
      }

      if (filter.ProjectStatus.HasValue)
      {
        query = query
          .Where(p => p.Status == (int)filter.ProjectStatus);
      }

      if (filter.IncludeDepartment)
      {
        query = query.Include(p => p.Department);
      }

      if (filter.DepartmentId.HasValue)
      {
        query = query.Where(p => p.Department.DepartmentId == filter.DepartmentId.Value);
      }

      if (filter.UserId.HasValue)
      {
        query = query
          .Where(p => p.Users
            .Any(u => u.IsActive && u.UserId == filter.UserId));
      }

      return query;
    }

    #endregion

    public ProjectRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public Task<DbProject> GetAsync(GetProjectFilter filter)
    {
      return filter is null
        ? null
        : CreateGetPredicate(filter).FirstOrDefaultAsync(p => p.Id == filter.ProjectId);
    }

    public async Task<List<DbProject>> GetAsync(IGetProjectsRequest request)
    {
      IQueryable<DbProject> projectsQuery = CreateGetPredicate(request);

      return await projectsQuery.ToListAsync();
    }

    public Task<DbProject> GetProjectWithUserAsync(Guid projectId, Guid userId)
    {
      return _provider.Projects.Include(x => x.Users.Where(user => user.UserId == userId && user.IsActive))
        .FirstOrDefaultAsync(project => project.Id == projectId);
    }

    public Task CreateAsync(DbProject dbProject)
    {
      if (dbProject is null)
      {
        return Task.CompletedTask;
      }

      _provider.Projects.Add(dbProject);
      return _provider.SaveAsync();
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
      if (filter is null)
      {
        return (null, 0);
      }

      IQueryable<DbProject> query = CreateFindPredicate(filter);

      int totalCount = await query.CountAsync();

      List<(DbProject dbProject, int usersCount)> dbProjects = 
        (await (from project in query.Skip(filter.SkipCount).Take(filter.TakeCount)
           select new
           {
             Project = project,
             UsersCount = _provider.ProjectsUsers.Count(pu => pu.ProjectId == project.Id && pu.IsActive)
           }).ToListAsync())
           .Select(p => (p.Project, p.UsersCount)).ToList();

      return (dbProjects, totalCount);
    }

    public async Task<List<DbProject>> SearchAsync(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return null;
      }

      return await _provider.Projects.Where(p => p.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) > -1 || p.ShortName.IndexOf(text, StringComparison.OrdinalIgnoreCase) > -1).ToListAsync();
    }

    public Task<bool> DoesExistAsync(Guid projectId)
    {
      return _provider.Projects.AnyAsync(x => x.Id == projectId);
    }

    public Task<bool> DoesNameExistAsync(string name, Guid? projectId = null)
    {
      return projectId.HasValue
        ? _provider.Projects.AnyAsync(p => p.Id != projectId.Value && p.Name.ToLower().Equals(name.ToLower()))
        : _provider.Projects.AnyAsync(p => p.Name.ToLower().Equals(name.ToLower()));
    }

    public Task<bool> DoesShortNameExistAsync(string shortName, Guid? projectId = null)
    {
      return projectId.HasValue
        ? _provider.Projects.AnyAsync(p => p.Id != projectId.Value && p.ShortName.ToLower().Equals(shortName.ToLower()))
        : _provider.Projects.AnyAsync(p => p.ShortName.ToLower().Equals(shortName.ToLower()));
    }

    public async Task<List<Guid>> DoExistAsync(List<Guid> projectsIds)
    {
      return await _provider.Projects.Where(p => projectsIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
    }
  }
}
