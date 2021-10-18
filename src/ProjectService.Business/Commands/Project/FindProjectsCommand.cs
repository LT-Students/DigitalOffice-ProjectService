using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
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
    private readonly IResponseCreater _responseCreater;

    private async Task<List<DepartmentData>> GetDepartmentsAsync(List<Guid> departmentsIds, List<string> errors)
    {
      if (departmentsIds == null || !departmentsIds.Any())
      {
        return null;
      }

      List<DepartmentData> departmentDatas = await _redisHelper.GetAsync<List<DepartmentData>>(Cache.Departments, departmentsIds.GetRedisCacheHashCode());

      if (departmentDatas != null)
      {
        _logger.LogInformation("Departments were taken from the cache. Departments ids: {departmentsIds}", string.Join(", ", departmentsIds));

        return departmentDatas;
      }

      return await GetDepartmentsThroughBrokerAsync(departmentsIds, errors);
    }

    private async Task<List<DepartmentData>> GetDepartmentsThroughBrokerAsync(List<Guid> departmentsIds, List<string> errors)
    {
      if (departmentsIds == null || !departmentsIds.Any())
      {
        return null;
      }

      string errorMessage = "Cannot get departments now. Please try again later.";

      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> response = await _rcGetDepartments
          .GetResponse<IOperationResult<IGetDepartmentsResponse>>(
            IGetDepartmentsRequest.CreateObj(departmentsIds));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Departments were taken from the service. Departments ids: {departmentsIds}", string.Join(", ", departmentsIds));

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
      IResponseCreater responseCreater)
    {
      _logger = logger;
      _repository = repository;
      _findFilterValidator = findFilterValidator;
      _responseMapper = responseMapper;
      _rcGetDepartments = rcGetDepartments;
      _redisHelper = redisHelper;
      _responseCreater = responseCreater;
    }

    public async Task<FindResultResponse<ProjectInfo>> ExecuteAsync(FindProjectsFilter filter)
    {
      if (!_findFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreater.CreateFailureFindResponse<ProjectInfo>(HttpStatusCode.BadRequest, errors);
      }

      (List<DbProject> dbProjects, int totalCount) = await _repository.FindAsync(filter);

      // TODO cut departments
      List<DepartmentData> departments = await GetDepartmentsAsync(
        dbProjects.Where(p => p.DepartmentId.HasValue).Select(p => p.DepartmentId.Value).ToList(), errors);

      FindResultResponse<ProjectInfo> response = _responseMapper.Map(dbProjects, totalCount, departments, errors);

      return response;
    }
  }
}
