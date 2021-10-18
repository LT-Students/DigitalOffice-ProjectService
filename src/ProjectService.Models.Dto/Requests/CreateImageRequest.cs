using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateImageRequest
  {
    public Guid ProjectId { get; set; }
    public List<ImageContent> Images { get; set; }
  }
}
