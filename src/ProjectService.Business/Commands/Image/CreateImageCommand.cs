﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task
{
    public class CreateImageCommand : ICreateImageCommand
    {
        private readonly IImageRepository _repository;
        private readonly IRequestClient<ICreateImageRequest> _rcImages;
        private readonly ILogger<CreateImageCommand> _logger;
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private List<Guid> CreateImage(List<CreateImageData> images, List<string> errors)
        {
            if (images == null || images.Count == 0)
            {
                return null;
            }

            string errorMessage = "Can not get images. Please try again later.";
            const string logMessage = "Errors while creating images.";

            try
            {
                Response<IOperationResult<ICreateImagesResponse>> brokerResponse = _rcImages.GetResponse<IOperationResult<ICreateImagesResponse>>(
                   ICreateImageRequest.CreateObj(images, ImageSource.Project)).Result;

                if (brokerResponse.Message.IsSuccess && brokerResponse.Message.Body != null)
                {
                    return brokerResponse.Message.Body.ImagesIds;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage);
            }

            errors.Add(errorMessage);

            return null;
        }

        public CreateImageCommand(
           IImageRepository repository,
           IRequestClient<ICreateImageRequest> rcImages,
           ILogger<CreateImageCommand> logger,
           IAccessValidator accessValidator,
           IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _rcImages = rcImages;
            _logger = logger;
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<bool> Execute(CreateImageRequest request)
        {
            OperationResultResponse<bool> response = new();

            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("Not enough rights.");

                return response;
            }

            List<CreateImageData> images = new List<CreateImageData>
            {
                new CreateImageData(request.Name, request.Content, request.Extension, _httpContextAccessor.HttpContext.GetUserId())
            };

            List<Guid> imagesIds = null;
            if (request.Image == (int)ImageType.Project)
            {
                imagesIds = CreateImage(images, response.Errors);
            }

            if (response.Errors.Any())
            {
                response.Status = OperationResultStatusType.Failed;
                return response;
            }

            response.Body = _repository.Create(imagesIds.Select(imageId => new DbProjectImage
            {
                Id = Guid.NewGuid(),
                ImageId = imageId,
                ProjectId = request.Id
            }).ToList());

            response.Status = OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}
