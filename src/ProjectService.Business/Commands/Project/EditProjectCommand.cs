using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class EditProjectCommand : IEditProjectCommand
  {
    private readonly IEditProjectValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IPatchDbProjectMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreateProjectCommand> _logger;

    private DepartmentData GetDepartment(Guid? departmentId, List<string> errors)
    {
      string errorMessage = "Cannot edit project. Please try again later.";

      if (!departmentId.HasValue)
      {
        return null;
      }

      try
      {
        var response = _rcGetDepartments.GetResponse<IOperationResult<IGetDepartmentsResponse>>(
        IGetDepartmentsRequest.CreateObj(new() { departmentId.Value })).Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Departments.FirstOrDefault();
        }

        _logger.LogWarning(
          "Can not find department with this id {departmentId}: " +
          "{Environment.NewLine}{string.Join('\n', response.Message.Errors)}", departmentId);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, errorMessage);

        errors.Add(errorMessage);
      }

      return null;
    }

    public EditProjectCommand(
      IEditProjectValidator validator,
      IAccessValidator accessValidator,
      IPatchDbProjectMapper mapper,
      IProjectRepository projectRepository,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IHttpContextAccessor httpContextAccessor,
      ILogger<CreateProjectCommand> logger)
    {
      _validator = validator;
      _accessValidator = accessValidator;
      _mapper = mapper;
      _projectRepository = projectRepository;
      _rcGetDepartments = rcGetDepartments;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
    }

    public OperationResultResponse<bool> Execute(Guid projectId, JsonPatchDocument<EditProjectRequest> request)
    {
      _validator.ValidateAndThrowCustom(request);

      DbProject dbProject = _projectRepository.Get(new GetProjectFilter { ProjectId = projectId });

      OperationResultResponse<bool> response = new();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveProjects))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      foreach (Operation item in request.Operations)
      {
        if (item.path == $"/{nameof(EditProjectRequest.Name)}" &&
          _projectRepository.DoesProjectNameExist(item.value.ToString()))
        {
          response.Status = OperationResultStatusType.Failed;
          response.Errors.Add($"Project with name '{item.value}' already exists.");
          return response;
        }

        if (item.path == $"/{nameof(EditProjectRequest.DepartmentId)}")
        {
          // TODO rework to department existence
          var departmentData = GetDepartment(Guid.Parse(item.value.ToString()), response.Errors);

          if (!response.Errors.Any() && departmentData == null)
          {
            _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            response.Status = OperationResultStatusType.Failed;
            response.Errors.Add("Project department does not found.");

            return response;
          }
          else if (response.Errors.Any())
          {
            _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            response.Status = OperationResultStatusType.Failed;
            return response;
          }
        };
      }

      response.Body = _projectRepository.Edit(dbProject, _mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
