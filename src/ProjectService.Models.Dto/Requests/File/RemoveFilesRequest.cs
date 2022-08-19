using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.File
{
  public record RemoveFilesRequest
  {
    public Guid ProjectId { get; set; }
    public List<Guid> FilesIds { get; set; }
  }
}
