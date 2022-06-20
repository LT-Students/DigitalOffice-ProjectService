using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateFilesRequest
  {
    public Guid ProjectId { get; set; }
    
    [Required]
    public List<FileInfo> Files { get; set; }
  }
}
