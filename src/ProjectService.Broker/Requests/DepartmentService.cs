using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class DepartmentService : IDepartmentService
  {
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly ILogger<DepartmentService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public DepartmentService(
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      ILogger<DepartmentService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcGetDepartments = rcGetDepartments;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<List<DepartmentData>> GetDepartmentsAsync(
      List<string> errors,
      List<Guid> departmentsIds = null,
      List<Guid> usersIds = null)
    {
      string key;

      if (usersIds is not null && usersIds.Any())
      {
        key = usersIds.GetRedisCacheHashCode(departmentsIds.FirstOrDefault());
      }
      else if (departmentsIds is not null && departmentsIds.Any())
      {
        key = departmentsIds.GetRedisCacheHashCode();
      }
      else
      {
        return null;
      }

      List<DepartmentData> departments = await _globalCache
        .GetAsync<List<DepartmentData>>(Cache.Departments, key);

      if (departments is not null)
      {
        _logger.LogInformation(
          $"Department was taken from the cache.");
      }
      else
      {
        departments = (await _rcGetDepartments.ProcessRequest<IGetDepartmentsRequest, IGetDepartmentsResponse>(
            IGetDepartmentsRequest.CreateObj(departmentsIds: departmentsIds, usersIds: usersIds),
            errors,
            _logger))
          ?.Departments;
      }

      return departments;
    }
  }
}
