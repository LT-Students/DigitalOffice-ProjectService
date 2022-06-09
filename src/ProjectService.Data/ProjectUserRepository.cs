using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class ProjectUserRepository : IProjectUserRepository
  {
    private readonly IDataProvider _provider;
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

    #endregion

    public ProjectUserRepository(
      IDataProvider provider,
      IHttpContextAccessor contextAccessor)
    {
      _provider = provider;
      _contextAccessor = contextAccessor;
    }

    public async Task<List<DbProjectUser>> GetAsync(List<Guid> usersIds)
    {
      return await _provider.ProjectsUsers.Where(u => usersIds.Contains(u.UserId) && u.IsActive).ToListAsync();
    }

    public async Task<List<DbProjectUser>> GetExistingUsersAsync(Guid projectId, IEnumerable<Guid> usersIds)
    {
      return await _provider.ProjectsUsers
        .Where(pu => pu.ProjectId == projectId && usersIds.Contains(pu.UserId)).ToListAsync();
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

    public async Task<bool> CreateAsync(List<DbProjectUser> newUsers)
    {
      if (newUsers is null)
      {
        return false;
      }

      _provider.ProjectsUsers.AddRange(newUsers);

      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> EditIsActiveAsync(List<DbProjectUser> dbUsers, Guid createdBy)
    {
      if (dbUsers is null)
      {
        return false;
      }

      foreach (DbProjectUser oldUser in dbUsers)
      {
        oldUser.IsActive = true;
        oldUser.CreatedBy = createdBy;
      }

      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> DoesExistAsync(Guid userId, Guid projectId, bool? isManager = null)
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

    public async Task<List<Guid>> DoExistAsync(Guid projectId, IEnumerable<Guid> usersIds, bool? isActive = null)
    {
      if (usersIds is null)
      {
        return default;
      }

      IQueryable<DbProjectUser> dbUsers = _provider.ProjectsUsers.Where(pu => pu.ProjectId == projectId && usersIds.Contains(pu.UserId));

      if (isActive.HasValue)
      {
        dbUsers = dbUsers.Where(x => x.IsActive == isActive.Value);
      }

      return await dbUsers.Select(x => x.UserId).ToListAsync();
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
        dbProjectUser.CreatedBy = removedBy;

        projectsIds.Add(dbProjectUser.ProjectId);
      }

      await _provider.SaveAsync();

      return projectsIds;
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

    public async Task<bool> EditAsync(Guid projectId, EditProjectUsersRoleRequest request)
    {
      await _provider.ProjectsUsers
        .Where(pu => pu.ProjectId == projectId && request.UsersIds.Contains(pu.UserId))
        .ForEachAsync(pu =>
        {
          pu.Role = (int)request.Role;
          pu.CreatedBy = _contextAccessor.HttpContext.GetUserId();
        });

      await _provider.SaveAsync();

      return true;
    }
  }
}
