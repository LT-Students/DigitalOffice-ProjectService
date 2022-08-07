using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image
{
  public record RemoveImageRequest
  {
    public Guid ProjectId { get; set; }
    public List<Guid> ImagesIds { get; set; }
  }
}
