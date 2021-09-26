using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateImageRequest
  {
    public Guid EntityId { get; set; }
    public List<ImageContent> Images { get; set; }
    public ImageType ImageType { get; set; }
  }
}
