using System;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
  public class ProjectResponse
  {
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public ProjectStatusType Status { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public string Customer { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DepartmentInfo Department { get; set; }
    public IEnumerable<ProjectUserInfo> Users { get; set; }
  }
}
