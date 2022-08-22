using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record ProjectInfo
  {
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public ProjectStatusType Status { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string ShortDescription { get; set; }
    public string Customer { get; set; }
    public int UsersCount { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DepartmentInfo Department { get; set; }
  }
}
