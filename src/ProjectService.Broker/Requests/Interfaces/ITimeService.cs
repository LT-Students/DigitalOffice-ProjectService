using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  public interface ITimeService
  {
    Task CreateWorkTimeAsync(Guid projectId, List<Guid> userIds, List<string> errors);
  }
}
