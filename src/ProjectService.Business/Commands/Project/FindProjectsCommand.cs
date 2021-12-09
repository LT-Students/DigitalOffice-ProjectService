using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class FindProjectsCommand : IFindProjectsCommand
  {
    private readonly ILogger<FindProjectsCommand> _logger;
    private readonly IProjectRepository _repository;
    private readonly IBaseFindFilterValidator _findFilterValidator;
    private readonly IFindProjectsResponseMapper _responseMapper;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IRedisHelper _redisHelper;
    private readonly IResponseCreator _responseCreator;

    private async Task<List<DepartmentData>> GetDepartmentsAsync(List<Guid> projectsIds, List<string> errors)
    {
      if (projectsIds == null || !projectsIds.Any())
      {
        return null;
      }

      List<DepartmentData> departmentDatas = await _redisHelper.GetAsync<List<DepartmentData>>(Cache.Departments, projectsIds.GetRedisCacheHashCode());

      if (departmentDatas != null)
      {
        _logger.LogInformation("Departments were taken from the cache for project with ids: {projectsIds}", string.Join(", ", projectsIds));

        return departmentDatas;
      }

      return await GetDepartmentsThroughBrokerAsync(projectsIds, errors);
    }

    private async Task<List<DepartmentData>> GetDepartmentsThroughBrokerAsync(List<Guid> projectsIds, List<string> errors)
    {
      if (projectsIds == null || !projectsIds.Any())
      {
        return null;
      }

      string errorMessage = "Cannot get departments now. Please try again later.";

      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> response = await _rcGetDepartments
          .GetResponse<IOperationResult<IGetDepartmentsResponse>>(
            IGetDepartmentsRequest.CreateObj(projectsIds: projectsIds));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Departments were taken from the service for project with ids: {projectsIds}", string.Join(", ", projectsIds));

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
      IBaseFindFilterValidator findFilterValidator,
      IFindProjectsResponseMapper responseMapper,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRedisHelper redisHelper,
      IResponseCreator responseCreator)
    {
      _logger = logger;
      _repository = repository;
      _findFilterValidator = findFilterValidator;
      _responseMapper = responseMapper;
      _rcGetDepartments = rcGetDepartments;
      _redisHelper = redisHelper;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<ProjectInfo>> ExecuteAsync(FindProjectsFilter filter)
    {
      if (!_findFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<ProjectInfo>(HttpStatusCode.BadRequest, errors);
      }

      (List<DbProject> dbProjects, int totalCount) = await _repository.FindAsync(filter);

      List<DepartmentData> departments = await GetDepartmentsAsync(
        dbProjects.Select(p => p.Id).ToList(), errors);

      FindResultResponse<ProjectInfo> response = _responseMapper.Map(dbProjects, totalCount, departments, errors);

      return response;
    }
  }
}
