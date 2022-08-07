using System;
using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.File
{
  public record FindProjectFilesFilter : BaseFindFilter
  {
    [FromQuery(Name = "projectid")]
    public Guid ProjectId { get; set; }
  }
}
