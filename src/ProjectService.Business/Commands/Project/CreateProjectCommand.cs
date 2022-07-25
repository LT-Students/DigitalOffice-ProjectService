using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class CreateProjectCommand : ICreateProjectCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IDbProjectMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateProjectRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IFileDataMapper _fileDataMapper;
    private readonly IImageService _imageService;
    private readonly IMessageService _messageService;
    private readonly IPublish _publish;

    public CreateProjectCommand(
      IProjectRepository repository,
      ICreateProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IDbProjectMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IFileDataMapper fileDataMapper,
      IImageService imageService,
      IMessageService messageService,
      IPublish publish)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _fileDataMapper = fileDataMapper;
      _imageService = imageService;
      _messageService = messageService;
      _publish = publish;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateProjectRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      List<Guid> imagesIds = await _imageService.CreateImagesAsync(request.ProjectImages, response.Errors);

      List<FileAccess> accesses = new List<FileAccess>();
      List<FileData> files = request.Files.Select(x => _fileDataMapper.Map(x, accesses)).ToList();

      DbProject dbProject = _mapper.Map(request, imagesIds, accesses);

      response.Body = await _repository.CreateAsync(dbProject);

      if (response.Body is null)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      List<Guid> usersIds = request.Users.Select(u => u.UserId).ToList();

      await Task.WhenAll(
        request.DepartmentId.HasValue
          ? _publish.CreateDepartmentEntityAsync(
              departmentId: request.DepartmentId.Value,
              createdBy: _httpContextAccessor.HttpContext.GetUserId(),
              projectId: response.Body.Value)
          : Task.CompletedTask,
        usersIds.Any()
          ? _publish.CreateWorkTimeAsync(
              dbProject.Id,
              usersIds)
          : Task.CompletedTask,
        usersIds.Any()
          ? _messageService.CreateWorkspaceAsync(request.Name, usersIds, response.Errors)
          : Task.CompletedTask,
        request.Files.Any()
          ? _publish.CreateFilesAsync(files)
          : Task.CompletedTask);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
