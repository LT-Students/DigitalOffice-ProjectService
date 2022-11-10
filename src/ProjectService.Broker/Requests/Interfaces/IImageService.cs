using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IImageService
  {
    Task<List<Guid>> CreateImagesAsync(List<ImageContent> projectImages, List<string> errors = null);
  }
}
