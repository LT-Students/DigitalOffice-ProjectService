using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class ProjectExpandedResponse
    {
        public ProjectInfo Project { get; set; }
        public DepartmentInfo Department { get; set; }
        public IEnumerable<ProjectUserInfo> Users { get; set; }
        public IEnumerable<ProjectFileInfo> Files { get; set; }
    }
}
