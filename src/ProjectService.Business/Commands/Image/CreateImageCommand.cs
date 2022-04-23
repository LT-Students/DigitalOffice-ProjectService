using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image
{
  public class CreateImageCommand : ICreateImageCommand
  {
    private readonly IImageRepository _repository;
    private readonly IRequestClient<ICreateImagesRequest> _rcImages;
    private readonly ILogger<CreateImageCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbImageMapper _dbProjectImageMapper;
    private readonly ICreateImagesRequestValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;

    private List<Guid> CreateImagesAsync(List<ImageContent> context, Guid userId, Guid enityId, List<string> errors)
    {
      List<CreateImageData> images = context
        .Select(x => new CreateImageData(x.Name, x.Content, x.Extension, userId))
        .ToList();

      string logMessage = $"Errors while creating images for project id {enityId}.";

      try
      {
        IOperationResult<ICreateImagesResponse> response =
          _rcImages.GetResponse<IOperationResult<ICreateImagesResponse>>(
            ICreateImagesRequest.CreateObj(images, ImageSource.Project)).Result.Message;

        if (response.IsSuccess && response.Body.ImagesIds != null)
        {
          return response.Body.ImagesIds;
        }

        _logger.LogWarning(
          logMessage + "Errors: { Errors}",
          string.Join('\n', response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      errors.Add("Can not create images. Please try again later.");

      return null;
    }

    public CreateImageCommand(
      IImageRepository repository,
      IRequestClient<ICreateImagesRequest> rcImages,
      ILogger<CreateImageCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDbImageMapper dbProjectImageMapper,
      ICreateImagesRequestValidator validator,
      IUserRepository userRepository,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _rcImages = rcImages;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _dbProjectImageMapper = dbProjectImageMapper;
      _validator = validator;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateImagesRequest request)
    {
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(await _userRepository.DoesExistAsync(request.ProjectId, userId, true)))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<List<Guid>>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string> { "Not enough rights." }
        };
      }
      
      ValidationResult validationResult = await _validator.ValidateAsync(request);
      
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(x => x.ErrorMessage)
          .ToList());
      }

      OperationResultResponse<List<Guid>> response = new();

      List<Guid> imagesIds = CreateImagesAsync(
        request.Images,
        userId,
        request.ProjectId,
        response.Errors);

      if (response.Errors.Any())
      {
        response.Status = OperationResultStatusType.Failed;
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return response;
      }

      response.Body = await _repository.CreateAsync(imagesIds.Select(imageId =>
        _dbProjectImageMapper.Map(request, imageId))
        .ToList());

      response.Status = OperationResultStatusType.FullSuccess;
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
