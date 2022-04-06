using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface ITimeService
  {
    Task CreateWorkTimeAsync(Guid projectId, List<Guid> userIds, List<string> errors);
  }
}
