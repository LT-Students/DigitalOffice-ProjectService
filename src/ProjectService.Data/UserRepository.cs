using System;
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

    public async Task<bool> RemoveAsync(Guid userId, Guid removedBy)
    {
      IQueryable<DbProjectUser> dbProjectsUser = _provider.ProjectsUsers.AsQueryable()
        .Where(u => u.UserId == userId && u.IsActive);

      foreach (DbProjectUser dbProjectUser in dbProjectsUser)
      {
        dbProjectUser.IsActive = false;
        dbProjectUser.ModifiedBy = removedBy;
        dbProjectUser.ModifiedAtUtc = DateTime.UtcNow;
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

    public async Task<bool> IsProjectAdminAsync(Guid projectId, Guid userId)
    {
      return await _provider.ProjectsUsers.AnyAsync(pu => pu.IsActive && pu.ProjectId == projectId && pu.UserId == userId);
    }
  }
}
