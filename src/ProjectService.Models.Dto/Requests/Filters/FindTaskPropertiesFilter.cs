using System;
using LT.DigitalOffice.Kernel.Validators.Models;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindTaskPropertiesFilter : BaseFindRequest
  {
    [FromQuery(Name = "name")]
    public string Name { get; set; }

    [FromQuery(Name = "projectid")]
    public Guid? ProjectId { get; set; }

    [FromQuery(Name = "authorid")]
    public Guid? AuthorId { get; set; }
  }
}
