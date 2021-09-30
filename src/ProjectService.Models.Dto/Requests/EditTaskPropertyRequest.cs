using System;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public class EditTaskPropertyRequest
  {
    public string Name { get; set; }
    public TaskPropertyType PropertyType { get; set; }
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
  }
}
