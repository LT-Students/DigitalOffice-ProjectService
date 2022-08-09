using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File
{
  public class EditFilesCommand : IEditFilesCommand
  {
    private readonly IProjectFileRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;

    public EditFilesCommand(
      IProjectFileRepository repository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IProjectUserRepository userRepository,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid fileId, FileAccessType accessType)
    {
      DbProjectFile file = (await _repository.GetAsync(new List<Guid>() { fileId })).FirstOrDefault();
      if (file is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      if (!await _userRepository.DoesExistAsync(userId: _httpContextAccessor.HttpContext.GetUserId(), projectId: file.ProjectId, isManager: true)
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new();
      response.Body = accessType != (FileAccessType)file.Access 
        ? await _repository.EditAsync(fileId, accessType)
        : true;

      if (!response.Body)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }

      return response;
    }
  }
}
