using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public record TaskInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public string PriorityName { get; set; }
        public int Number { get; set; }
        public int? PlannedMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserTaskInfo Author { get; set; }
        public ProjectTaskInfo Project { get; set; }
        public UserTaskInfo AssignedTo { get; set; }
    }
}
