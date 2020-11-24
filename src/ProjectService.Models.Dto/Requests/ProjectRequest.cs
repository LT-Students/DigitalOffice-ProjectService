using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class ProjectRequest
    {
        public Project Project { get; set; }
        public IEnumerable<ProjectUser> Users { get; set; }

        public ProjectRequest()
        {
            Project = new Project();
        }
    }
}
