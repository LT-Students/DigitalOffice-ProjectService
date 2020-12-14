using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class ProjectExpandedRequest
    {
        public ProjectRequest Project { get; set; }
        public IEnumerable<ProjectUserRequest> Users { get; set; }

        public ProjectExpandedRequest()
        {
            Project = new Project();
        }
    }
}
