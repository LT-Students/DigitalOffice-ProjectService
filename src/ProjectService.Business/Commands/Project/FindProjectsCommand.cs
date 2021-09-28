using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class FindProjectsCommand : IFindProjectsCommand
  {
    private readonly ILogger<FindProjectsCommand> _logger;
    private readonly IProjectRepository _repository;
    private readonly IFindProjectsResponseMapper _responseMapper;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IConnectionMultiplexer _cache;

    private async Task<List<DepartmentData>> GetDepartments(List<Guid> departmentsIds, List<string> errors)
    {
      if (departmentsIds == null || !departmentsIds.Any())
      {
        return null;
      }

      RedisValue departmentFromCache = await _cache.GetDatabase(Cache.Departments).StringGetAsync(departmentsIds.GetRedisCacheHashCode());

      if (departmentFromCache.HasValue)
      {
        return JsonConvert.DeserializeObject<List<DepartmentData>>(departmentFromCache);
      }

      return await GetDepartmentsThroughBroker(departmentsIds, errors);
    }

    private async Task<List<DepartmentData>> GetDepartmentsThroughBroker(List<Guid> departmentsIds, List<string> errors)
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
      IConnectionMultiplexer cache)
    {
      _logger = logger;
      _repository = repository;
      _responseMapper = responseMapper;
      _rcGetDepartments = rcGetDepartments;
      _cache = cache;
    }

    public async Task<FindResponse<ProjectInfo>> Execute(FindProjectsFilter filter, int skipCount, int takeCount)
    {
      if (filter == null)
      {
        throw new ArgumentNullException(nameof(filter));
      }

      List<string> errors = new();

      List<DbProject> dbProject = _repository.Find(filter, skipCount, takeCount, out int totalCount);

      List<DepartmentData> departments = await GetDepartments(
        dbProject.Where(p => p.DepartmentId.HasValue).Select(p => p.DepartmentId.Value).ToList(), errors);

      FindResponse<ProjectInfo> response = _responseMapper.Map(dbProject, totalCount, departments, errors);

      return response;
    }
  }
}
