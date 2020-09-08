using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto
{
    public class NewProjectRequest
    {
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
