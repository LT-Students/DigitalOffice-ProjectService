using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
  public class CreateTaskCommand : ICreateTaskCommand
  {
    private readonly ITaskRepository _repository;
    private readonly ICreateTaskValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IDbTaskMapper _mapperTask;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployee;
    private readonly IAccessValidator _accessValidator;
    private readonly ILogger<CreateTaskCommand> _logger;
    private readonly IRequestClient<ICreateImagesRequest> _rcImages;

    private DepartmentData GetDepartment(Guid authorId, List<string> errors)
    {
      string errorMessage = "Cannot create task. Please try again later.";

      try
      {
        var response = _rcGetCompanyEmployee.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
          IGetCompanyEmployeesRequest.CreateObj(new() { authorId }, includeDepartments: true)).Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Departments.FirstOrDefault();
        }

        _logger.LogWarning("Can not find department contain user with Id: '{authorId}'", authorId);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, errorMessage);

        errors.Add(errorMessage);
      }

      return null;
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

    public CreateTaskCommand(
      ITaskRepository repository,
      ICreateTaskValidator validator,
      IDbTaskMapper mapperTask,
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IUserRepository userRepository,
      IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployees,
      ILogger<CreateTaskCommand> logger,
      IRequestClient<ICreateImagesRequest> rcImages)
    {
      _repository = repository;
      _validator = validator;
      _mapperTask = mapperTask;
      _httpContextAccessor = httpContextAccessor;
      _rcGetCompanyEmployee = rcGetCompanyEmployees;
      _accessValidator = accessValidator;
      _logger = logger;
      _userRepository = userRepository;
      _rcImages = rcImages;
    }

    public OperationResultResponse<Guid> Execute(CreateTaskRequest request)
    {
      OperationResultResponse<Guid> response = new();

      Guid authorId = _httpContextAccessor.HttpContext.GetUserId();

      if (!_accessValidator.IsAdmin()
        && !_userRepository.AreUserProjectExist(authorId, request.ProjectId)
        && GetDepartment(authorId, response.Errors)?.DirectorUserId != authorId)
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

      List<Guid> imagesIds = CreateImage(request.TaskImages.ToList(), authorId, response.Errors);

      response.Body = _repository.Create(_mapperTask.Map(request, authorId, imagesIds));

      response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
