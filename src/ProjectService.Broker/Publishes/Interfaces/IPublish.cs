using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces
{
  [AutoInject]
  public interface IPublish
  {
    Task CreateDepartmentEntityAsync(Guid departmentId, Guid createdBy, Guid projectId);
    Task CreateWorkTimeAsync(Guid projectId, List<Guid> usersIds);
  }
}
