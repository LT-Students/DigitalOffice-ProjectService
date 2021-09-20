﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task
{
  public class CreateImageCommand : ICreateImageCommand
  {
    private readonly IImageRepository _repository;
    private readonly IRequestClient<ICreateImagesRequest> _rcImages;
    private readonly ILogger<CreateImageCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbImageMapper _dbProjectImageMapper;
    private readonly ICreateImageValidator _validator;

    private List<Guid> CreateImage(List<ImageContent> context, Guid userId, List<string> errors)
    {
      List<CreateImageData> images = context
        .Select(x => new CreateImageData(x.Name, x.Content, x.Extension, userId))
        .ToList();

      string logMessage = $"Errors while creating images for user id {userId}.";

      try
      {
        IOperationResult<ICreateImagesResponse> response = _rcImages.GetResponse<IOperationResult<ICreateImagesResponse>>(
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
      ICreateImageValidator validator)
    {
      _repository = repository;
      _rcImages = rcImages;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _dbProjectImageMapper = dbProjectImageMapper;
      _validator = validator;
    }

    public OperationResultResponse<List<Guid>> Execute(CreateImageRequest request)
    {
      OperationResultResponse<List<Guid>> response = new();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveProjects))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      List<Guid> imagesIds = CreateImage(
        request.Images,
        _httpContextAccessor.HttpContext.GetUserId(),
        response.Errors);

      if (response.Errors.Any())
      {
        response.Status = OperationResultStatusType.Failed;
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return response;
      }

      response.Body = _repository.Create(imagesIds.Select(imageId =>
        _dbProjectImageMapper.Map(request, imageId))
        .ToList());

      response.Status = OperationResultStatusType.FullSuccess;
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
