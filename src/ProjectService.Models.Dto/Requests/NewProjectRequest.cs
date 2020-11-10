using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class NewProjectRequest
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Guid DepartmentId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
