using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IImageService
  {
    Task<List<ImageInfo>> GetImagesAsync(List<Guid> imagesIds, ImageSource imageSource, List<string> errors);

    Task<List<Guid>> CreateImageAsync(List<ImageContent> projectImages, List<string> errors);

    Task<bool> RemoveImages(List<Guid> imagesIds, List<string> errors);
  }
}
