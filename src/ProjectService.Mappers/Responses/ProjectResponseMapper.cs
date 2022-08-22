using System.Linq;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses
{
  public class ProjectResponseMapper : IProjectResponseMapper
  {
    private readonly IProjectInfoMapper _projectMapper;

    public ProjectResponseMapper(IProjectInfoMapper projectMapper)
    {
      _projectMapper = projectMapper;
    }

    public ProjectResponse Map(
      DbProject dbProject,
      DepartmentInfo department)
    {
      return dbProject is null
        ? null
        : new ProjectResponse
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
          Users = dbProject.Users.Select(pu =>
            new ProjectUserInfo()
            {
              UserId = pu.UserId,
              Role = (ProjectUserRoleType)pu.Role
            })
        };
    }
  }
}
