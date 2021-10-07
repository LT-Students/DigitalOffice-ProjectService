﻿using System;
using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindTaskPropertiesFilter : BaseFindFilter
  {
    [FromQuery(Name = "name")]
    public string Name { get; set; }

    [FromQuery(Name = "projectid")]
    public Guid? ProjectId { get; set; }

    [FromQuery(Name = "authorid")]
    public Guid? AuthorId { get; set; }
  }
}
