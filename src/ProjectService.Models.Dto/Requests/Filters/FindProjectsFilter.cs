using System;
using LT.DigitalOffice.Kernel.Validators.Models;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindProjectsFilter : BaseFindRequest
  {
    [FromQuery(Name = "departmentid")]
    public Guid? DepartmentId { get; set; }
  }
}
