using System;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public class GetProjectFilter
  {
    [FromQuery(Name = "projectid")]
    public Guid ProjectId { get; set; }

    [FromQuery(Name = "includeusers")]
    public bool IncludeUsers { get; set; } = false;

    [FromQuery(Name = "shownotactiveusers")]
    public bool ShowNotActiveUsers { get; set; } = false;

    [FromQuery(Name = "includefiles")]
    public bool IncludeFiles { get; set; } = false;

    [FromQuery(Name = "includeimages")]
    public bool IncludeImages { get; set; } = false;
  }
}
