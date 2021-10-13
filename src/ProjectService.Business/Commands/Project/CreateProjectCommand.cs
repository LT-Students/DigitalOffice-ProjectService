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
    private readonly IDbProjectMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateProjectRequestValidator _validator;
    private readonly ILogger<CreateProjectCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateWorkspaceRequest> _rcCreateWorkspace;
    private readonly IRequestClient<ICreateWorkTimeRequest> _rcCreateWorkTime;
    private readonly IRequestClient<ICreateImagesRequest> _rcImages;

    private async System.Threading.Tasks.Task CreateWorkspace(string projectName, List<Guid> usersIds, List<string> errors)
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

    private async System.Threading.Tasks.Task CreateWorkTime(Guid projectId, List<Guid> userIds, List<string> errors)
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

    private async Task<List<Guid>> CreateImage(List<ImageContent> projectImages, List<string> errors)
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

    public CreateProjectCommand(
      IProjectRepository repository,
      ICreateProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IDbProjectMapper mapper,
      ILogger<CreateProjectCommand> logger,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateWorkspaceRequest> rcCreateWorkspace,
      IRequestClient<ICreateWorkTimeRequest> rcCreateWorkTime,
      IRequestClient<ICreateImagesRequest> rcImages)
    {
      _logger = logger;
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateWorkspace = rcCreateWorkspace;
      _rcCreateWorkTime = rcCreateWorkTime;
      _rcImages = rcImages;
    }

    public async Task<OperationResultResponse<Guid?>> Execute(CreateProjectRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<Guid?>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string> { "Not enough rights." }
        };
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<Guid?>
        {
          Status = OperationResultStatusType.Failed,
          Errors = validationResult.Errors.Select(vf => vf.ErrorMessage).ToList()
        };
      }

      OperationResultResponse<Guid?> response = new();

      List<Guid> imagesIds = await CreateImage(request.ProjectImages, response.Errors);

      DbProject dbProject = _mapper.Map(request, imagesIds);

      response.Body = _repository.Create(dbProject);

      if (response.Body == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        return response;
      }

      List<Guid> usersIds = request.Users.Select(u => u.UserId).ToList();

      await CreateWorkTime(dbProject.Id, usersIds, response.Errors);

      await CreateWorkspace(request.Name, usersIds, response.Errors);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      response.Status = response.Errors.Any() ?
        OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
