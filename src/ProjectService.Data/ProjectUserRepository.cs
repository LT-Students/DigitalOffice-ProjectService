﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class ProjectUserRepository : IProjectUserRepository
  {
    private readonly IDataProvider _provider;
    private readonly ILogger<ProjectUserRepository> _logger;
    private readonly IHttpContextAccessor _contextAccessor;

    #region private methods

    private IQueryable<DbProjectUser> CreateGetPredicate(IGetProjectsUsersRequest request)
    {
      IQueryable<DbProjectUser> projectUsers = _provider.ProjectsUsers.AsQueryable();

      if (request.UsersIds != null && request.UsersIds.Any())
      {
        projectUsers = projectUsers.Where(pu => request.UsersIds.Contains(pu.UserId));
      }

      if (request.ProjectsIds != null && request.ProjectsIds.Any())
      {
        projectUsers = projectUsers.Where(pu => request.ProjectsIds.Contains(pu.ProjectId));
      }

      if (!request.IncludeDisactivated)
      {
        projectUsers = projectUsers.Where(pu => pu.IsActive);
      }

      return projectUsers;
    }

    private static Dictionary<Guid, int> CreateUserIdByRoleTypeDictionary(
      IReadOnlyCollection<Guid> userIds,
      IReadOnlyCollection<ProjectUserRoleType> roleTypes)
    {
      Dictionary<Guid, int> userRoleTypes = new();

      for (int i = 0; i < userIds.Count(); i++)
      {
        userRoleTypes.Add(
          userIds.ElementAt(i),
          (int)roleTypes.ElementAt(i));
      }

      return userRoleTypes;
    }

    #endregion

    public ProjectUserRepository(
      IDataProvider provider,
      ILogger<ProjectUserRepository> logger,
      IHttpContextAccessor contextAccessor)
    {
      _provider = provider;
      _logger = logger;
      _contextAccessor = contextAccessor;
    }

    public async Task<List<DbProjectUser>> GetAsync(Guid projectId, bool showNotActive)
    {
      IQueryable<DbProjectUser> dbProjectQueryable = _provider.ProjectsUsers.AsQueryable();

      if (showNotActive)
      {
        dbProjectQueryable = dbProjectQueryable.Where(x => x.ProjectId == projectId);
      }
      else
      {
        dbProjectQueryable = dbProjectQueryable.Where(x => x.ProjectId == projectId && x.IsActive);
      }

      return await dbProjectQueryable.ToListAsync();
    }

    public async Task<List<DbProjectUser>> GetAsync(List<Guid> usersIds)
    {
      return await _provider.ProjectsUsers.Where(u => usersIds.Contains(u.UserId) && u.IsActive).ToListAsync();
    }

    public async Task<List<Guid>> GetExistAsync(Guid projectId, IEnumerable<Guid> usersIds)
    {
      return await _provider.ProjectsUsers
        .Where(pu => pu.IsActive && pu.ProjectId == projectId && usersIds.Contains(pu.UserId)).Select(pu => pu.UserId).ToListAsync();
    }

    public async Task<(List<DbProjectUser>, int totalCount)> GetAsync(IGetProjectsUsersRequest request)
    {
      IQueryable<DbProjectUser> projectUsers = CreateGetPredicate(request);

      int totalCount = await projectUsers.CountAsync();

      if (request.SkipCount.HasValue)
      {
        projectUsers = projectUsers.Skip(request.SkipCount.Value);
      }

      if (request.TakeCount.HasValue)
      {
        projectUsers = projectUsers.Take(request.TakeCount.Value);
      }

      return (await projectUsers.ToListAsync(), totalCount);
    }


    public async Task<bool> CreateAsync(List<DbProjectUser> dbProjectUsers)
    {
      if (dbProjectUsers == null)
      {
        return false;
      }

      _provider.ProjectsUsers.AddRange(dbProjectUsers);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> DoesExistAsync(Guid userId, Guid projectId, bool? isManager)
    {
      if (isManager.HasValue && isManager.Value)
      {
        return await _provider
          .ProjectsUsers
          .AnyAsync(x => x.UserId == userId && x.ProjectId == projectId && x.Role == (int)ProjectUserRoleType.Manager && x.IsActive);
      }

      return await _provider
        .ProjectsUsers
        .AnyAsync(x => x.UserId == userId && x.ProjectId == projectId && x.IsActive);
    }

    public async Task<List<Guid>> RemoveAsync(Guid userId, Guid removedBy)
    {
      List<DbProjectUser> dbProjectsUser = await _provider.ProjectsUsers
        .Where(u => u.UserId == userId && u.IsActive).ToListAsync();

      List<Guid> projectsIds = new();

      if (dbProjectsUser is null || !dbProjectsUser.Any())
      {
        return projectsIds;
      }

      foreach (DbProjectUser dbProjectUser in dbProjectsUser)
      {
        dbProjectUser.IsActive = false;
        dbProjectUser.ModifiedBy = removedBy;
        dbProjectUser.ModifiedAtUtc = DateTime.UtcNow;

        projectsIds.Add(dbProjectUser.ProjectId);
      }

      await _provider.SaveAsync();

      return projectsIds;
    }

    public async Task<List<Guid>> DoExistAsync(Guid projectId, List<Guid> ids)
    {
      return await _provider.ProjectsUsers
        .Where(pu => pu.ProjectId == projectId && ids.Contains(pu.UserId) && pu.IsActive)
        .Select(pu => pu.UserId)
        .ToListAsync();
    }

    public async Task<bool> RemoveAsync(Guid projectId, IEnumerable<Guid> usersIds)
    {
      List<DbProjectUser> users = await _provider.ProjectsUsers
        .Where(pu => pu.IsActive && pu.ProjectId == projectId && usersIds.Contains(pu.UserId)).ToListAsync();

      if (!users.Any())
      {
        return false;
      }

      foreach (var user in users)
      {
        user.IsActive = false;
      }

      _provider.ProjectsUsers.UpdateRange(users);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> EditProjectUsers(
      Guid projectId, 
      List<Guid> usersIds,
      List<ProjectUserRoleType> roleTypes)
    {
      Dictionary<Guid, int> userIdByRoleType = CreateUserIdByRoleTypeDictionary(usersIds, roleTypes);

      await _provider.ProjectsUsers
        .Where(pu => pu.ProjectId == projectId && usersIds.Contains(pu.UserId))
        .ForEachAsync(pu =>
        {
          pu.Role = userIdByRoleType[pu.UserId];
          pu.ModifiedBy = _contextAccessor.HttpContext.GetUserId();
          pu.ModifiedAtUtc = DateTime.Now;
        });

      return true;
    }

    public async Task<bool> IsProjectAdminAsync(Guid projectId, Guid userId)
    {
      return await _provider.ProjectsUsers.AnyAsync(pu => pu.IsActive && pu.ProjectId == projectId && pu.UserId == userId);
    }
  }
}