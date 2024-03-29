﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  /// <summary>
  /// Represents interface of repository in repository pattern.
  /// Provides methods for working with the database of ProjectService.
  /// </summary>
  [AutoInject]
  public interface IProjectUserRepository
  {
    Task<(List<DbProjectUser>, int totalCount)> GetAsync(IGetProjectsUsersRequest request);

    Task<List<DbProjectUser>> GetExistingUsersAsync(Guid projectId, IEnumerable<Guid> usersIds);

    Task<ProjectUserRoleType?> GetUserRoleAsync(Guid projectId, Guid userId);

    Task<List<DbProjectUser>> GetAsync(List<Guid> usersIds);

    Task<List<DbProjectUser>> GetAsync(Guid projectId, bool? isActive, CancellationToken cancellationToken);

    Task CreateAsync(List<DbProjectUser> newUsers);

    Task<bool> EditIsActiveAsync(List<DbProjectUser> oldUsers, Guid createdBy, List<UserRequest> usersRoles = null);

    Task<bool> DoesExistAsync(Guid userId, Guid projectId, bool? isManager = null);

    Task<List<Guid>> DoExistAsync(Guid projectId, IEnumerable<Guid> usersIds, bool? isActive = null);

    Task<List<Guid>> RemoveAsync(Guid userId, Guid removedBy);

    Task<bool> RemoveAsync(Guid projectId, IEnumerable<Guid> usersIds);

    Task<bool> EditAsync(Guid projectId, EditProjectUsersRoleRequest request);
  }
}
