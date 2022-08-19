using System;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project
{
  public class GetProjectFilter
  {
    [FromQuery(Name = "projectid")]
    public Guid ProjectId { get; set; }

    [FromQuery(Name = "includedepartment")]
    public bool IncludeDepartment { get; set; } = false;

    [FromQuery(Name = "includeprojectusers")]
    public bool IncludeProjectUsers { get; set; } = false;
  }
}
