using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
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

        private bool RemoveImage(List<Guid> ids, List<string> errors)
        {
            if (ids == null || ids.Count == 0)
            {
                return false;
            }

            string errorMessage = "Can not get images. Please try again later.";
            const string logMessage = "Errors while creating images.";

            try
            {
                Response<IOperationResult<bool>> brokerResponse = _rcImages.GetResponse<IOperationResult<bool>>(
                   IRemoveImagesRequest.CreateObj(ids, ImageSource.Project)).Result;

                if (brokerResponse.Message.IsSuccess)
                {
                    return true;
                }
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
           IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _rcImages = rcImages;
            _logger = logger;
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<bool> Execute(RemoveImageRequest request)
        {
            OperationResultResponse<bool> response = new();

            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("Not enough rights.");

                return response;
            }

            List<Guid> imagesIds = new List<Guid>
            {
                request.Id
            };

            bool result = RemoveImage(imagesIds, response.Errors);

            if (response.Errors.Any() && !result)
            {
                response.Status = OperationResultStatusType.Failed;
                return response;
            }

            response.Body = _repository.Remove(request.Id);

            response.Status = OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}
