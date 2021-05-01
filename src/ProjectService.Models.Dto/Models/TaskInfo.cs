using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class TaskInfo
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PriorityId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public int? PlannedMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
