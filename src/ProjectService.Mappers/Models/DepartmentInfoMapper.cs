using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class DepartmentInfoMapper : IDepartmentInfoMapper
  {
    public DepartmentInfo Map(DepartmentData department)
    {
      if (department is null)
      {
        return null;
      }

      return new DepartmentInfo
      {
        Id = department.Id,
        Name = department.Name,
        ShortName = department.ShortName
      };
    }
  }
}
