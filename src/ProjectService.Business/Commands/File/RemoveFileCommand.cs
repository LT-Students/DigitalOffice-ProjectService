using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
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
  public class RemoveFileCommand : IRemoveFileCommand
  {
    private readonly IFileRepository _repository;
    private readonly IRequestClient<IRemoveFilesRequest> _rcFiles;
    private readonly ILogger<RemoveFileCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    private async Task<bool> RemoveFileAsync(List<Guid> ids, List<string> errors)
    {
      if (ids == null || !ids.Any())
      {
        return false;
      }

      string logMessage = "Errors while removing files ids {ids}. Errors: {Errors}";

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcFiles.GetResponse<IOperationResult<bool>>(
            IRemoveFilesRequest.CreateObj(ids));

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

      errors.Add("Can not remove files. Please try again later.");

      return false;
    }

    public RemoveFileCommand(
      IFileRepository repository,
      IRequestClient<IRemoveFilesRequest> rcFiles,
      ILogger<RemoveFileCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository)
    {
      _repository = repository;
      _rcFiles = rcFiles;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveFilesRequest request)
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

      OperationResultResponse<bool> response = new();

      bool result = await RemoveFileAsync(request.FilesIds, response.Errors);

      if (!result)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      response.Body = await _repository.RemoveAsync(request.FilesIds);
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
