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
          Project = _projectMapper.Map(dbProject, dbProject.Users.Count, department),
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
