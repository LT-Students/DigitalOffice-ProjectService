using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Department;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IDepartmentService
  {
    Task<List<DepartmentData>> GetDepartmentsAsync(
      List<Guid> departmentsIds = null,
      List<Guid> usersIds = null,
      List<string> errors = null);

    Task<DepartmentUserRole?> GetDepartmentUserRoleAsync(Guid departmentId, Guid userId, List<string> errors = null);
  }
}
