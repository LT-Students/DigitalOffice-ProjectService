using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
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
    private readonly IDepartmentService _departmentService;
    private readonly IImageService _imageService;
    private readonly IFileService _fileService;
    private readonly ITimeService _timeService;
    private readonly IMessageService _messageService;

    public CreateProjectCommand(
      IProjectRepository repository,
      ICreateProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IDbProjectMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IFileDataMapper fileDataMapper,
      IDepartmentService departmentService,
      IImageService imageService,
      IFileService fileService,
      ITimeService timeService,
      IMessageService messageService)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _fileDataMapper = fileDataMapper;
      _departmentService = departmentService;
      _imageService = imageService;
      _fileService = fileService;
      _timeService = timeService;
      _messageService = messageService;
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

      List<Guid> imagesIds = await _imageService.CreateImageAsync(request.ProjectImages, response.Errors);

      List<FileAccess> accesses = new List<FileAccess>();
      List<FileData> files = request.Files?.Select(x => _fileDataMapper.Map(x, accesses)).ToList();

      DbProject dbProject = await _fileService.CreateFilesAsync(files, response.Errors) ?
        _mapper.Map(request, imagesIds, accesses) :
        _mapper.Map(request, imagesIds, null);

      response.Body = await _repository.CreateAsync(dbProject);

      if (response.Body == null)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      List<Guid> usersIds = request.Users.Select(u => u.UserId).ToList();

      await Task.WhenAll(
        _departmentService.CreateDepartmentEntityAsync(request.DepartmentId, dbProject.Id, response.Errors),
        _timeService.CreateWorkTimeAsync(dbProject.Id, usersIds, response.Errors),
        _messageService.CreateWorkspaceAsync(request.Name, usersIds, response.Errors));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      response.Status = response.Errors.Any() ?
        OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
