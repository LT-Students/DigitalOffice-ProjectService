﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image
{
  public class RemoveImageCommand : IRemoveImageCommand
  {
    private readonly IImageRepository _repository;
    private readonly ILogger<RemoveImageCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRemoveImagesRequestValidator _validator;
    private readonly IProjectUserRepository _userRepository;
    private readonly IImageService _imageService;
    private readonly IResponseCreator _responseCreator;
    private readonly IPublish _publish;

    public RemoveImageCommand(
      IImageRepository repository,
      ILogger<RemoveImageCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IRemoveImagesRequestValidator validator,
      IProjectUserRepository userRepository,
      IImageService imageService,
      IResponseCreator responseCreator,
      IPublish publish)
    {
      _repository = repository;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _validator = validator;
      _userRepository = userRepository;
      _imageService = imageService;
      _responseCreator = responseCreator;
      _publish = publish;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveImageRequest request)
    {
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(await _userRepository.DoesExistAsync(request.ProjectId, userId, true)))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          errors);
      }

      OperationResultResponse<bool> response = new();

      bool result = await _imageService.RemoveImagesAsync(request.ImagesIds, response.Errors);

      if (!result)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, response.Errors);
      }

      response.Body = await _repository.RemoveAsync(request.ImagesIds);

      if (response.Body)
      {
        await _publish.RemoveImagesAsync(request.ImagesIds, ImageSource.Project);
      }

      return response;
    }
  }
}
