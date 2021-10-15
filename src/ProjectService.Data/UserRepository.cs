﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class UserRepository : IUserRepository
  {
    private readonly IDataProvider _provider;

    #region private methods

    private IQueryable<DbProjectUser> CreateGetPredicates(
      FindDbProjectsUserFilter filter,
      IQueryable<DbProjectUser> dbProjectUser)
    {
      if (filter.UserId.HasValue)
      {
        dbProjectUser = dbProjectUser.Where(x => x.UserId == filter.UserId);
      }

      if (filter.IncludeProject.HasValue && filter.IncludeProject.Value)
      {
        dbProjectUser = dbProjectUser.Include(x => x.Project);
      }

      return dbProjectUser;
    }

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

    #endregion

    public UserRepository(IDataProvider provider)
    {
      _provider = provider;
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

    public async Task<List<DbProjectUser>> GetAsync(List<Guid> userIds)
    {
      return await _provider.ProjectsUsers.Where(u => userIds.Contains(u.UserId) && u.IsActive).ToListAsync();
    }

    public async Task<List<Guid>> GetExistAsync(Guid projectId, IEnumerable<Guid> userIds)
    {
      return await _provider.ProjectsUsers
        .Where(pu => pu.IsActive && pu.ProjectId == projectId && userIds.Contains(pu.UserId)).Select(pu => pu.UserId).ToListAsync();
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

    public async Task<List<DbProjectUser>> FindAsync(FindDbProjectsUserFilter filter)
    {
      if (filter == null)
      {
        return null;
      }

      var dbProjectsUser = _provider.ProjectsUsers.AsQueryable();

      return await CreateGetPredicates(filter, dbProjectsUser).ToListAsync();
    }

    public async Task<(List<DbProjectUser>, int totalCount)> FindAsync(Guid projectId, int? skipCount, int? takeCount)
    {
      IQueryable<DbProjectUser> users = _provider.ProjectsUsers.Where(pu => pu.ProjectId == projectId && pu.IsActive).AsQueryable();

      int totalCount = await users.CountAsync();

      if (skipCount.HasValue)
      {
        users = users.Skip(skipCount.Value);
      }

      if (takeCount.HasValue)
      {
        users = users.Take(takeCount.Value);
      }

      return (await users.ToListAsync(), totalCount);
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

    public async Task<bool> RemoveAsync(Guid userId, Guid removedBy)
    {
      List<DbProjectUser> users = await _provider.ProjectsUsers.Where(u => u.UserId == userId && u.IsActive).ToListAsync();

      foreach (var user in users)
      {
        user.IsActive = false;
        user.ModifiedBy = removedBy;
        user.ModifiedAtUtc = DateTime.UtcNow;
      }

      await _provider.SaveAsync();

      return true;
    }

    public async Task<List<Guid>> DoExistAsync(Guid projectId, List<Guid> ids)
    {
      return await _provider.ProjectsUsers
        .Where(pu => pu.ProjectId == projectId && ids.Contains(pu.UserId) && pu.IsActive)
        .Select(pu => pu.UserId)
        .ToListAsync();
    }

    public async Task<bool> RemoveAsync(Guid projectId, IEnumerable<Guid> userIds)
    {
      List<DbProjectUser> users = await _provider.ProjectsUsers
        .Where(pu => pu.IsActive && pu.ProjectId == projectId && userIds.Contains(pu.UserId)).ToListAsync();

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
  }
}
