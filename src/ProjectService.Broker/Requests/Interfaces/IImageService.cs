using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IImageService
  {
    Task<List<ImageInfo>> GetImagesAsync(
      List<Guid> imagesIds, ImageSource imageSource, List<string> errors = null, CancellationToken cancellationToken = default);

    Task<List<Guid>> CreateImagesAsync(List<ImageContent> projectImages, List<string> errors = null);
  }
}
