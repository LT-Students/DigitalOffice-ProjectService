using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public class RemoveImageRequest
  {
    public Guid EntityId { get; set; }
    public ImageType ImageType { get; set; }
    public List<Guid> ImagesIds { get; set; }
  }
}
