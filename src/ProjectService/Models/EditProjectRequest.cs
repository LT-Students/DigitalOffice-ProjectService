using System;

namespace LT.DigitalOffice.ProjectService.Models
{
    public class EditProjectRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }
}