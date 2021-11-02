using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class CreateProjectCommand : ICreateProjectCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IDbProjectMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateProjectRequestValidator _validator;
    private readonly ILogger<CreateProjectCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateDepartmentEntityRequest> _rcCreateDepartmentEntity;
    private readonly IRequestClient<ICreateWorkspaceRequest> _rcCreateWorkspace;
    private readonly IRequestClient<ICreateWorkTimeRequest> _rcCreateWorkTime;
    private readonly IRequestClient<ICreateImagesRequest> _rcImages;
    private readonly IRequestClient<ICreateFilesRequest> _rcFiles;
    private readonly IResponseCreater _responseCreater;

    #region private methods

    private async Task CreateWorkspaceAsync(string projectName, List<Guid> usersIds, List<string> errors)
    {
      string errorMessage = $"Failed to create a workspace for the project {projectName}";
      string logMessage = "Cannot create workspace for project {name}";

      Guid creatorId = _httpContextAccessor.HttpContext.GetUserId();

      try
      {
        if (!usersIds.Contains(creatorId))
        {
          usersIds.Add(creatorId);
        }

        Response<IOperationResult<bool>> response =
          await _rcCreateWorkspace.GetResponse<IOperationResult<bool>>(
            ICreateWorkspaceRequest.CreateObj(
              projectName,
              creatorId,
              usersIds));

        if (!(response.Message.IsSuccess && response.Message.Body))
        {
          _logger.LogWarning(logMessage, projectName);
          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, projectName);

        errors.Add(errorMessage);
      }
    }

    private async Task CreateWorkTimeAsync(Guid projectId, List<Guid> userIds, List<string> errors)
    {
      string errorMessage = $"Failed to create a work time for project {projectId} with users: {string.Join(", ", userIds)}.";
      const string logMessage = "Failed to create a work time for project {projectId} with users {userIds}";

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcCreateWorkTime.GetResponse<IOperationResult<bool>>(
            ICreateWorkTimeRequest.CreateObj(projectId, userIds));

        if (!(response.Message.IsSuccess && response.Message.Body))
        {
          _logger.LogWarning(logMessage, projectId, string.Join(", ", userIds));
          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, projectId, string.Join(", ", userIds));

        errors.Add(errorMessage);
      }
    }

    private async Task<List<Guid>> CreateImageAsync(List<ImageContent> projectImages, List<string> errors)
    {
      if (projectImages == null || !projectImages.Any())
      {
        return null;
      }

      string logMessage = "Errors while creating images. Errors: {Errors}";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> response =
          await _rcImages.GetResponse<IOperationResult<ICreateImagesResponse>>(
            ICreateImagesRequest.CreateObj(
              projectImages.Select(x => new CreateImageData(
                x.Name,
                x.Content,
                x.Extension,
                _httpContextAccessor.HttpContext.GetUserId()))
              .ToList(),
              ImageSource.Project));

        if (response.Message.IsSuccess && response.Message.Body.ImagesIds != null)
        {
          return response.Message.Body.ImagesIds;
        }

        _logger.LogWarning(
          logMessage,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      errors.Add("Can not create images. Please try again later.");

      return null;
    }

    private async Task CreateDepartmentEntityAsync(Guid projectId, Guid? departmentId, List<string> errors)
    {
      if (!departmentId.HasValue)
      {
        return;
      }

      string logMessage = "Unable to enroll project {projectId} in the department {departmentId}.";

      try
      {
        Response<IOperationResult<bool>> response = await _rcCreateDepartmentEntity.GetResponse<IOperationResult<bool>>(
          ICreateDepartmentEntityRequest.CreateObj(
            departmentId: departmentId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            projectId: projectId));

        if (response.Message.IsSuccess && response.Message.Body)
        {
          return;
        }

        _logger.LogWarning(logMessage, projectId, departmentId);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, projectId, departmentId);
      }

      errors.Add("Unable to enroll project in the department. Please try again later.");
    }

    private async Task<bool> CreateFileAsync(List<FileData> files, List<string> errors)
    {
      if (files == null || !files.Any())
      {
        return false;
      }

      string logMessage = "Errors while creating files. Errors: {Errors}";

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcFiles.GetResponse<IOperationResult<bool>>(
            ICreateFilesRequest.CreateObj(
              files,
              _httpContextAccessor.HttpContext.GetUserId()));

        if (response.Message.IsSuccess)
        {
          return true;
        }

        _logger.LogWarning(
          logMessage,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      errors.Add("Can not create files. Please try again later.");

      return false;
    }

    #endregion

    public CreateProjectCommand(
      IProjectRepository repository,
      ICreateProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IDbProjectMapper mapper,
      ILogger<CreateProjectCommand> logger,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateDepartmentEntityRequest> rcCreateDepartmentEntity,
      IRequestClient<ICreateWorkspaceRequest> rcCreateWorkspace,
      IRequestClient<ICreateWorkTimeRequest> rcCreateWorkTime,
      IRequestClient<ICreateImagesRequest> rcImages,
      IRequestClient<ICreateFilesRequest> rcFiles,
      IResponseCreater responseCreater)
    {
      _logger = logger;
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateDepartmentEntity = rcCreateDepartmentEntity;
      _rcCreateWorkspace = rcCreateWorkspace;
      _rcCreateWorkTime = rcCreateWorkTime;
      _rcImages = rcImages;
      _responseCreater = responseCreater;
      _rcFiles = rcFiles;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateProjectRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        return _responseCreater.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreater.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      List<Guid> imagesIds = await CreateImageAsync(request.ProjectImages, response.Errors);

      List<FileData> files = request.Files.Select(x =>
        new FileData(Guid.NewGuid(),
          x.Name,
          x.Content,
          x.Extension)).ToList();

      DbProject dbProject = await CreateFileAsync(files, response.Errors) ?
         _mapper.Map(request, imagesIds, files.Select(x => x.Id).ToList()) :
         _mapper.Map(request, imagesIds, null);

      if (await CreateFileAsync(files, response.Errors))
      {
        dbProject = _mapper.Map(request, imagesIds, files.Select(x => x.Id).ToList());
      }
      else
      {
        dbProject = _mapper.Map(request, imagesIds, null);
      }

      response.Body = await _repository.CreateAsync(dbProject);

      if (response.Body == null)
      {
        return _responseCreater.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      List<Guid> usersIds = request.Users.Select(u => u.UserId).ToList();

      await Task.WhenAll(
        CreateDepartmentEntityAsync(dbProject.Id, request.DepartmentId, response.Errors),
        CreateWorkTimeAsync(dbProject.Id, usersIds, response.Errors),
        CreateWorkspaceAsync(request.Name, usersIds, response.Errors));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      response.Status = response.Errors.Any() ?
        OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
