using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class ProjectRequest
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public Guid? DepartmentId { get; set; }
        public ProjectStatusType Status { get; set; }
        public IEnumerable<ProjectUserRequest> Users { get; set; }
    }
}
