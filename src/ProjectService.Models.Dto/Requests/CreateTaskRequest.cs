using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class CreateTaskRequest
    {
        //public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public string Description { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid TypeId { get; set; }
        public Guid StatusId { get; set; }
        public Guid PriorityId { get; set; }
        public DateTime? Deadline { get; set; }
        public int? PlannedMinutes { get; set; }
        public Guid? ParentTaskId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Number { get; set; }
    }
}
