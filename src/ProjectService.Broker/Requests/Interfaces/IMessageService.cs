using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IMessageService
  {
    Task CreateWorkspaceAsync(string projectName, List<Guid> usersIds, List<string> errors);
  }
}
