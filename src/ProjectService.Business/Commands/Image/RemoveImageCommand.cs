﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

        private bool RemoveImage(List<Guid> ids, List<string> errors)
        {
            if (ids == null || ids.Count == 0)
            {
                return false;
            }

            string errorMessage = "Can not remove images. Please try again later.";
            const string logMessage = "Errors while removing images.";

            try
            {
                Response<IOperationResult<bool>> brokerResponse = _rcImages.GetResponse<IOperationResult<bool>>(
                   IRemoveImagesRequest.CreateObj(ids, ImageSource.Project)).Result;

                if (brokerResponse.Message.IsSuccess && brokerResponse.Message.Body)
                {
                    return true;
                }

                _logger.LogWarning(
                    errorMessage,
                    string.Join('\n', brokerResponse.Message.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage);
            }

            errors.Add(errorMessage);

            return false;
        }

        public RemoveImageCommand(
           IImageRepository repository,
           IRequestClient<IRemoveImagesRequest> rcImages,
           ILogger<RemoveImageCommand> logger,
           IAccessValidator accessValidator,
           IHttpContextAccessor httpContextAccessor,
           IRemoveImageValidator validator)
        {
            _repository = repository;
            _rcImages = rcImages;
            _logger = logger;
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public OperationResultResponse<bool> Execute(List<RemoveImageRequest> request)
        {
            OperationResultResponse<bool> response = new();
            List<string> errors = new();

            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("Not enough rights.");

                return response;
            }

            if (!_validator.ValidateCustom(request, out errors))
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = OperationResultStatusType.Failed;
                response.Errors.AddRange(errors);

                return response;
            }

            List<Guid> imagesIds = request
                .Select(x => x.ImageId)
                .ToList();

            bool result = RemoveImage(imagesIds, response.Errors);

            if (response.Errors.Any() && !result)
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = OperationResultStatusType.Failed;

                return response;
            }

            response.Body = _repository.Remove(imagesIds);
            response.Status = OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}
