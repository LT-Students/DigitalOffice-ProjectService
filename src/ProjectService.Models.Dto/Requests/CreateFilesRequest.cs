using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateFilesRequest
  {
    public Guid ProjectId { get; set; }
    public AccessType Access { get; set; }
    public List<FileContent> Files { get; set; }
  }
}
