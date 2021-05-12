using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces
{
    [AutoInject]
    public interface IProjectExpandedResponseMapper
    {
        ProjectExpandedResponse Map(DbProject dbProject, IEnumerable<ProjectUserInfo> users, IEnumerable<ProjectFileInfo> files, DepartmentInfo department, List<string> errors);
    }
}
