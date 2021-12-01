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
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File
{
  public class RemoveFilesCommand : IRemoveFilesCommand
  {
    private readonly IFileRepository _repository;
    private readonly IRequestClient<IRemoveFilesRequest> _rcFiles;
    private readonly ILogger<RemoveFilesCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;

    private async Task<bool> RemoveFilesAsync(List<Guid> ids, List<string> errors)
    {
      if (ids == null || !ids.Any())
      {
        return false;
      }

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcFiles.GetResponse<IOperationResult<bool>>(
            IRemoveFilesRequest.CreateObj(ids));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }

        _logger.LogWarning(
          "Errors while removing files ids {ids}.\nErrors: {Errors}",
          string.Join(',', ids),
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc,
          "Errors while removing files ids {ids}.",
          string.Join('\n', ids));
      }

      errors.Add("Can not remove files. Please try again later.");

      return false;
    }

    public RemoveFilesCommand(
      IFileRepository repository,
      IRequestClient<IRemoveFilesRequest> rcFiles,
      ILogger<RemoveFilesCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _rcFiles = rcFiles;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveFilesRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(await _userRepository.DoesExistAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId(), true)))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new();

      bool result = await RemoveFilesAsync(request.FilesIds, response.Errors);

      if (!result)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, response.Errors);
      }

      response.Body = await _repository.RemoveAsync(request.FilesIds);
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
