using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class TaskResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public int? PlannedMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public DbProject Project { get; set; }
        public DbProjectUser Author { get; set; }
        public DbProjectUser AssignedUser { get; set; }
        public DbTaskProperty Status { get; set; }
        public DbTaskProperty Priority { get; set; }
        public DbTaskProperty Type { get; set; }
        public DbTask ParentTask { get; set; }

        public ICollection<DbTask> Subtasks { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}
