using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces
{
  [AutoInject]
  public interface IPublishHelper
  {
    Task CreateDepartmentEntityPublish(Guid departmentId, Guid createdBy, Guid? userId = null, Guid? projectId = null);
    Task CreateWorkTimePublish(Guid projectId, List<Guid> usersIds);
  }
}
