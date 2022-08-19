using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IProjectDepartmentDataMapper
  {
    ProjectDepartmentData Map(DbProjectDepartment dbProjectDepartment);
  }
}
