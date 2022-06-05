using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

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

    Task<List<DbProjectUser>> GetAsync(List<Guid> usersIds);

    Task<bool> CreateAsync(List<DbProjectUser> newUsers, List<DbProjectUser> oldUsers);

    Task<bool> DoesExistAsync(Guid userId, Guid projectId, bool? isManager = null);

    Task<List<Guid>> DoExistAsync(Guid projectId, List<Guid> ids);

    Task<List<Guid>> RemoveAsync(Guid userId, Guid removedBy);

    Task<bool> RemoveAsync(Guid projectId, IEnumerable<Guid> usersIds);

    Task<bool> EditAsync(ProjectUsersRequest request);

    Task<bool> IsProjectAdminAsync(Guid projectId, Guid userId);
  }
}
