using LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestModels;
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
