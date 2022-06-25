using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Department;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IDepartmentService
  {
    Task<List<DepartmentData>> GetDepartmentsAsync(
      List<string> errors,
      List<Guid> projectsIds = null,
      List<Guid> usersIds = null);
  }
}
