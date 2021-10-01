using System;
using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindTasksFilter : BaseFindFilter
  {
    [FromQuery(Name = "number")]
    public int? Number { get; set; }

    [FromQuery(Name = "projectid")]
    public Guid? ProjectId { get; set; }

    [FromQuery(Name = "assignedto")]
    public Guid? AssignedTo { get; set; }

    [FromQuery(Name = "status")]
    public Guid? Status { get; set; }
  }
}
