using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  /// <summary>
  /// Represents interface of repository in repository pattern.
  /// Provides methods for working with the database of ProjectService.
  /// </summary>
  [AutoInject]
  public interface IUserRepository
  {
    Task<(List<DbProjectUser>, int totalCount)> GetAsync(IGetProjectsUsersRequest request);

    Task<List<Guid>> GetExistAsync(Guid projectId, IEnumerable<Guid> userIds);

    Task<bool> CreateAsync(List<DbProjectUser> dbProjectUsers);

    Task<List<DbProjectUser>> FindAsync(FindDbProjectsUserFilter filter);

    Task<List<DbProjectUser>> GetAsync(List<Guid> userIds);

    Task<bool> DoesExistAsync(Guid userId, Guid projectId, bool? isManager = null);

    Task<List<Guid>> DoExistAsync(Guid projectId, List<Guid> ids);

    Task<bool> RemoveAsync(Guid userId, Guid removedBy);

    Task<bool> RemoveAsync(Guid projectId, IEnumerable<Guid> userIds);
  }
}
