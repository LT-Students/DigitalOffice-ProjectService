using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class TaskResponse
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

        public ICollection<DbTask> Subtasks { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}