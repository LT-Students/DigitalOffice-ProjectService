using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
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

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task
{
    public class CreateImageCommand : ICreateImageCommand
    {
        private IImageRepository _repository;
        private readonly IRequestClient<ICreateImagesProjectRequest> _rcImages;
        private readonly ILogger<CreateImageCommand> _logger;
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private List<Guid> CreateImage(List<CreateImageData> images, List<string> errors)
        {
            if (images == null || images.Count == 0)
            {
                return new();
            }

            string errorMessage = "Can not get images. Please try again later.";
            const string logMessage = "Errors while creating images.";

            try
            {
                Response<IOperationResult<ICreateImagesResponse>> brokerResponse = _rcImages.GetResponse<IOperationResult<ICreateImagesResponse>>(
                   ICreateImagesProjectRequest.CreateObj(images)).Result;

                if (brokerResponse.Message.IsSuccess && brokerResponse.Message.Body != null)
                {
                    return brokerResponse.Message.Body.ImageIds;
                }

                _logger.LogWarning(
                    logMessage,
                    string.Join('\n', brokerResponse.Message.Errors));

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage);
            }

            errors.Add(errorMessage);

            return new();
        }

        public CreateImageCommand(
           IImageRepository repository,
           IRequestClient<ICreateImagesProjectRequest> rcImages,
           ILogger<CreateImageCommand> logger,
           IAccessValidator accessValidator,
           IHttpContextAccessor httpContextAccessor
            )

        {
            _repository = repository;
            _rcImages = rcImages;
            _logger = logger;
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<bool> Execute(CreateImageRequest request)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            OperationResultResponse<bool> response = new();

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
