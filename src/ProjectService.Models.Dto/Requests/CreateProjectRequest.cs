﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateProjectRequest
  {
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public string Customer { get; set; }
    public DateTime? StartProject { get; set; }
    public DateTime? EndProject { get; set; }
    public Guid? DepartmentId { get; set; }
    public ProjectStatusType Status { get; set; }
    public List<ImageContent> ProjectImages { get; set; }
    public List<FileInfo> Files { get; set; }

    [Required]
    public List<UserRequest> Users { get; set; }
  }
}
