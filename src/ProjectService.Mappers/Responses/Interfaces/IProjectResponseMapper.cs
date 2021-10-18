using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IProjectResponseMapper
  {
    ProjectResponse Map(
      DbProject dbProject,
      IEnumerable<UserInfo> users,
      IEnumerable<ProjectFileInfo> files,
      IEnumerable<ImageInfo> images,
      DepartmentInfo department);
  }
}
