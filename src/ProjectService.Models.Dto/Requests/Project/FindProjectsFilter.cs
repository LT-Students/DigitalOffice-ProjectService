﻿using System;
using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.Models.Broker.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project
{
  public record FindProjectsFilter : BaseFindFilter
  {
    [FromQuery(Name = "isascendingsort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "projectstatus")]
    public ProjectStatusType? ProjectStatus { get; set; }

    [FromQuery(Name = "nameincludesubstring")]
    public string NameIncludeSubstring { get; set; }

    [FromQuery(Name = "includeDepartment")]
    public bool IncludeDepartment { get; set; } = false;

    [FromQuery(Name = "userid")]
    public Guid? UserId { get; set; }

    [FromQuery(Name = "departmentid")]
    public Guid? DepartmentId { get; set; }
  }
}
