using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record DepartmentInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
  }
}
