using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class EditTaskRequest
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid PriorityId { get; set; }
        public Guid StatusId { get; set; }
        public Guid TypeId { get; set; }
        public int? PlannedMinutes { get; set; }
    }
}
