using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image
{
  public record CreateImagesRequest
  {
    public Guid ProjectId { get; set; }
    public List<ImageContent> Images { get; set; }
  }
}
