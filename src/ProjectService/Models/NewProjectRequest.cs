using System;

namespace LT.DigitalOffice.ProjectService.Models
{
    public class NewProjectRequest
    {
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
