using System;
using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindProjectsFilter : BaseFindFilter
  {
    [FromQuery(Name = "departmentid")]
    public Guid? DepartmentId { get; set; }
  }
}
