using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public class CreateTaskRequest
  {
    public string Name { get; set; }
    public Guid ProjectId { get; set; }
    public string Description { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid TypeId { get; set; }
    public Guid StatusId { get; set; }
    public Guid PriorityId { get; set; }
    public int? PlannedMinutes { get; set; }
    public Guid? ParentId { get; set; }
    public List<ImageContent> TaskImages { get; set; }
  }
}
