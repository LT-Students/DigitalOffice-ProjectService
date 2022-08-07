using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IUserService
  {
    Task<(List<UserData> usersData, int totalCount)> GetFilteredUsersAsync(List<Guid> usersIds, FindProjectUsersFilter filter);

    Task<List<UserData>> GetUsersDatasAsync(IEnumerable<DbProjectUser> projectUsers, List<string> errors);

    Task<List<Guid>> CheckUsersExistenceAsync(List<Guid> usersIds, List<string> errors = null);
  }
}
