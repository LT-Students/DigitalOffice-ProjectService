using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
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
    private readonly IRemoveImagesRequestValidator _validator;
    private readonly IUserRepository _userRepository;

    private async Task<bool> RemoveImageAsync(List<Guid> ids, List<string> errors)
    {
      if (ids == null || !ids.Any())
      {
        return false;
      }

      string logMessage = "Errors while removing images ids {ids}. Errors: {Errors}";

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcImages.GetResponse<IOperationResult<bool>>(
            IRemoveImagesRequest.CreateObj(ids, ImageSource.Project));

        if (response.Message.IsSuccess)
        {
          return true;
        }

        _logger.LogWarning(
          logMessage,
          string.Join('\n', ids),
          string.Join('\n', response.Message.Errors));
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
      IRemoveImagesRequestValidator validator,
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

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveImageRequest request)
    {
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(await _userRepository.DoesExistAsync(request.ProjectId, userId, true)))
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

      bool result = await RemoveImageAsync(request.ImagesIds, response.Errors);

      if (!result)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      response.Body = await _repository.RemoveAsync(request.ImagesIds);
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
