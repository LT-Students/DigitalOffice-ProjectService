using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  [AutoInject]
  public interface IProjectDepartmentRepository
  {
    Task CreateAsync(DbProjectDepartment request);

    Task<bool> EditAsync(Guid projectId, Guid? departmentId);
  }
}
