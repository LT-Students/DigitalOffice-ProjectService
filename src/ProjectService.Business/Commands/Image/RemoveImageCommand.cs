﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image
{
  public class RemoveImageCommand : IRemoveImageCommand
  {
    private readonly IImageRepository _repository;
    private readonly IRequestClient<IRemoveImagesRequest> _rcImages;
    private readonly ILogger<RemoveImageCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRemoveImageValidator _validator;
    private readonly IUserRepository _userRepository;

    private bool RemoveImage(List<Guid> ids, List<string> errors)
    {
      if (ids == null || !ids.Any())
      {
        return false;
      }

      string logMessage = "Errors while removing images ids {ids}. Errors: {Errors}";

      try
      {
        IOperationResult<bool> response = _rcImages.GetResponse<IOperationResult<bool>>(
          IRemoveImagesRequest.CreateObj(ids, ImageSource.Project)).Result.Message;

        if (response.IsSuccess)
        {
          return true;
        }

        _logger.LogWarning(
          logMessage,
          string.Join('\n', ids),
          string.Join('\n', response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      errors.Add("Can not remove images. Please try again later.");

      return false;
    }

    public RemoveImageCommand(
      IImageRepository repository,
      IRequestClient<IRemoveImagesRequest> rcImages,
      ILogger<RemoveImageCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IRemoveImageValidator validator,
      IUserRepository userRepository)
    {
      _repository = repository;
      _rcImages = rcImages;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _validator = validator;
      _userRepository = userRepository;
    }

    public async Task<OperationResultResponse<bool>> Execute(RemoveImageRequest request)
    {
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(request.ImageType == ImageType.Project && _userRepository.AreUserProjectExist(request.ProjectId, userId, true)))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string> { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      OperationResultResponse<bool> response = new();

      bool result = RemoveImage(request.ImagesIds, response.Errors);

      if (!result)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      response.Body = _repository.Remove(request.ImagesIds);
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
