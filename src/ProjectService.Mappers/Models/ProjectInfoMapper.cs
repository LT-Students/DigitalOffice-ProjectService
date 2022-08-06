using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class ProjectInfoMapper : IProjectInfoMapper
  {
    public ProjectInfo Map(DbProject dbProject, int usersCount, DepartmentInfo department)
    {
      if (dbProject == null)
      {
        return null;
      }

      return new ProjectInfo
      {
        Id = dbProject.Id,
        Name = dbProject.Name,
        CreatedBy = dbProject.CreatedBy,
        Status = (ProjectStatusType)dbProject.Status,
        CreatedAtUtc = dbProject.CreatedAtUtc,
        ShortName = dbProject.ShortName,
        Description = dbProject.Description,
        ShortDescription = dbProject.ShortDescription,
        Department = department,
        StartDateUtc = dbProject.StartDateUtc,
        EndDateUtc = dbProject.EndDateUtc,
        Customer = dbProject.Customer,
        UsersCount = usersCount
      };
    }
  }
}

