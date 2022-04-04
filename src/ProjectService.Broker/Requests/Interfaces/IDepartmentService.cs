using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Department;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IDepartmentService
  {
    Task CreateDepartmentEntityAsync(Guid? departmentId, Guid projectId, List<string> errors);

    Task<List<DepartmentData>> GetDepartmentsAsync(Guid projectId, List<Guid> usersIds, List<string> errors);
  }
}
