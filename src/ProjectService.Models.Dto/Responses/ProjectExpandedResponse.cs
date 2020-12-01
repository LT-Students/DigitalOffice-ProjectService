using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class ProjectExpandedResponse
    {
        public Project Project { get; set; }
        public Department Department { get; set; }
        public IEnumerable<ProjectUser> Users { get; set; }
        public IEnumerable<ProjectFile> Files { get; set; }
    }
}
