using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Department
{
  public class EditProjectDepartmentRequest
  {
    public Guid ProjectId { get; set; }
    public Guid? DepartmentId { get; set; }
  }
}
