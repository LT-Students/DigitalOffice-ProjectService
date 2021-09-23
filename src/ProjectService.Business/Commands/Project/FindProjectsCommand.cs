using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class FindProjectsCommand : IFindProjectsCommand
  {
    private readonly ILogger<FindProjectsCommand> _logger;
    private readonly IProjectRepository _repository;
    private readonly IFindProjectsResponseMapper _responseMapper;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBaseFindRequestValidator _findRequestValidator;

    private List<DepartmentData> GetDepartments(List<DbProject> dbProjects, List<string> errors)
    {
      if (dbProjects == null || !dbProjects.Any())
      {
        return null;
      }

      List<Guid> departmentIds = dbProjects.Where(p => p.DepartmentId.HasValue).Select(p => p.DepartmentId.Value).ToList();

      if (!departmentIds.Any())
      {
        return null;
      }

      string errorMessage = "Cannot get departments now. Please try again later.";

      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> response = _rcGetDepartments
          .GetResponse<IOperationResult<IGetDepartmentsResponse>>(
            IGetDepartmentsRequest.CreateObj(departmentIds)).Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Departments;
        }

        _logger.LogWarning(string.Join(", ", response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, errorMessage);
      }

      errors.Add(errorMessage);

      return null;
    }

    public FindProjectsCommand(
      ILogger<FindProjectsCommand> logger,
      IProjectRepository repository,
      IFindProjectsResponseMapper responseMapper,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IHttpContextAccessor httpContextAccessor,
      IBaseFindRequestValidator findRequestValidator)
    {
      _logger = logger;
      _repository = repository;
      _responseMapper = responseMapper;
      _rcGetDepartments = rcGetDepartments;
      _httpContextAccessor = httpContextAccessor;
      _findRequestValidator = findRequestValidator;
    }

    public FindResultResponse<ProjectInfo> Execute(FindProjectsFilter filter)
    {
      FindResultResponse<ProjectInfo> response = new();

      if (_findRequestValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Concat(errors);

        return response;
      }

      List<DbProject> dbProject = _repository.Find(filter, out int totalCount);

      List<DepartmentData> departments = GetDepartments(dbProject, response.Errors.ToList());

      response = _responseMapper.Map(dbProject, totalCount, departments, response.Errors.ToList());

      return response;
    }
  }
}
