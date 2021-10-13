using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public class RemoveImageRequest
  {
    public Guid ProjectId { get; set; }
    public ImageType ImageType { get; set; }
    public List<Guid> ImagesIds { get; set; }
  }
}
