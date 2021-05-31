using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class TaskPropertyInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AuthorId { get; set; }
        public int PropertyType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}