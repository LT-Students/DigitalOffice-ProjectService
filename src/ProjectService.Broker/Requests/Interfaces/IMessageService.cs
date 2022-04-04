using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  public interface IMessageService
  {
    Task CreateWorkspaceAsync(string projectName, List<Guid> usersIds, List<string> errors);
  }
}
