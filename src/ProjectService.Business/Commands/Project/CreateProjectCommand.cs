using System;
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
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class CreateProjectCommand : ICreateProjectCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IDbProjectMapper _dbProjectMapper;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateProjectValidator _validator;
    private readonly ILogger<CreateProjectCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateWorkspaceRequest> _rcCreateWorkspace;
    private readonly IRequestClient<ICreateWorkTimeRequest> _rcCreateWorkTime;
    private readonly IRequestClient<ICreateImagesRequest> _rcImages;

    private void CreateWorkspace(string projectName, Guid creatorId, List<Guid> users, List<string> errors)
    {
      string errorMessage = $"Failed to create a workspace for the project {projectName}";
      string logMessage = "Cannot create workspace for project {name}";

      try
      {
        if (!users.Contains(creatorId))
        {
          users.Add(creatorId);
        }

        var response = _rcCreateWorkspace.GetResponse<IOperationResult<bool>>(
          ICreateWorkspaceRequest.CreateObj(projectName, creatorId, users), timeout: RequestTimeout.Default).Result;

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

    private void CreateWorkTime(Guid projectId, List<Guid> userIds, List<string> errors)
    {
      string errorMessage = $"Failed to create a work time for project {projectId} with users: {string.Join(", ", userIds)}.";
      const string logMessage = "Failed to create a work time for project {projectId} with users {userIds}";

      try
      {
        var response = _rcCreateWorkTime.GetResponse<IOperationResult<bool>>(
          ICreateWorkTimeRequest.CreateObj(projectId, userIds)).Result;

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

    private List<Guid> CreateImage(List<ImageContent> projectImages, Guid userId, List<string> errors)
    {
      if (projectImages == null || projectImages.Count == 0)
      {
        return null;
      }

      string logMessage = "Errors while creating images. Errors: {Errors}";

      try
      {
        IOperationResult<ICreateImagesResponse> response = _rcImages.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            projectImages.Select(x => new CreateImageData(x.Name, x.Content, x.Extension, userId)).ToList(),
            ImageSource.Project)).Result.Message;

        if (response.IsSuccess && response.Body.ImagesIds != null)
        {
          return response.Body.ImagesIds;
        }

        _logger.LogWarning(
          logMessage,
          string.Join('\n', response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      errors.Add("Can not create images. Please try again later.");

      return null;
    }

    public CreateProjectCommand(
      IProjectRepository repository,
      ICreateProjectValidator validator,
      IAccessValidator accessValidator,
      IDbProjectMapper dbProjectMapper,
      ILogger<CreateProjectCommand> logger,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateWorkspaceRequest> rcCreateWorkspace,
      IRequestClient<ICreateWorkTimeRequest> rcCreateWorkTime,
      IRequestClient<ICreateImagesRequest> rcImages)
    {
      _logger = logger;
      _validator = validator;
      _repository = repository;
      _dbProjectMapper = dbProjectMapper;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateWorkspace = rcCreateWorkspace;
      _rcCreateWorkTime = rcCreateWorkTime;
      _rcImages = rcImages;
    }

    public OperationResultResponse<Guid> Execute(CreateProjectRequest request)
    {
      OperationResultResponse<Guid> response = new();

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

      List<Guid> users = request.Users.Select(u => u.UserId).ToList();

      Guid userId = _httpContextAccessor.HttpContext.GetUserId();

      List<Guid> imagesIds = CreateImage(request.ProjectImages, userId, response.Errors);

      DbProject dbProject = _dbProjectMapper.Map(
        request,
        userId,
        users,
        new List<Guid> { request.DepartmentId.GetValueOrDefault() },
        imagesIds);

      response.Body = _repository.Create(dbProject);

      CreateWorkTime(dbProject.Id, users, response.Errors);

      CreateWorkspace(request.Name, userId, users, response.Errors);

      response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
