using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class ProjectDepartmentDataMapper : IProjectDepartmentDataMapper
  {
    public ProjectDepartmentData Map(DbProjectDepartment dbProjectDepartment)
    {
      if (dbProjectDepartment is null || !dbProjectDepartment.IsActive)
      {
        return null;
      }

      return new(
        dbProjectDepartment.ProjectId,
        dbProjectDepartment.DepartmentId);
    }
  }
}
