using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  public interface IUserService
  {
    Task<List<UserData>> GetUsersDatasAsync(IEnumerable<DbProjectUser> projectUsers, List<string> errors);
  }
}
