using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class ImageService : IImageService
  {
    private readonly ILogger<ImageService> _logger;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImages;
    private readonly IRequestClient<IRemoveImagesRequest> _rcRemoveImages;
    private readonly IImageInfoMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ImageService(
      ILogger<ImageService> logger,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<ICreateImagesRequest> rcCreateImages,
      IRequestClient<IRemoveImagesRequest> rcRemoveImages,
      IImageInfoMapper mapper,
      IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _rcCreateImages = rcCreateImages;
      _rcGetImages = rcGetImages;
      _rcRemoveImages = rcRemoveImages;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ImageInfo>> GetImagesAsync(List<Guid> imagesIds, ImageSource imageSource, List<string> errors)
    {
      if (imagesIds is null || !imagesIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
          _rcGetImages,
          IGetImagesRequest.CreateObj(imagesIds, imageSource),
          errors,
          _logger))
        ?.ImagesData
        .Select(_mapper.Map).ToList();

    }

    public async Task<List<Guid>> CreateImageAsync(List<ImageContent> projectImages, List<string> errors)
    {
      return projectImages is null || !projectImages.Any()
        ? null
        : (await RequestHandler
          .ProcessRequest<ICreateImagesRequest, ICreateImagesResponse>(
            _rcCreateImages,
            ICreateImagesRequest.CreateObj(
              projectImages.Select(x => new CreateImageData(
                x.Name,
                x.Content,
                x.Extension,
                _httpContextAccessor.HttpContext.GetUserId()))
              .ToList(),
              ImageSource.Project),
            errors,
            _logger)).ImagesIds;
    }

    public async Task<bool> RemoveImages(List<Guid> imagesIds, List<string> errors)
    {
      return imagesIds is null || imagesIds.Any()
        ? false
        : (await RequestHandler
          .ProcessRequest<IRemoveImagesRequest, bool>(
            _rcRemoveImages,
            IRemoveImagesRequest.CreateObj(imagesIds: imagesIds, imageSource: ImageSource.Project)),
            errors,
            _logger).Item1;
    }
  }
}
