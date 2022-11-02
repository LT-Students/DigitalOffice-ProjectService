using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
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
    private readonly IRequestClient<IGetDepartmentUserRoleRequest> _rcGetDepartmentUserRole;

    public DepartmentService(
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      ILogger<DepartmentService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IGetDepartmentUserRoleRequest> rcGetDepartmentUserRole)
    {
      _rcGetDepartments = rcGetDepartments;
      _logger = logger;
      _globalCache = globalCache;
      _rcGetDepartmentUserRole = rcGetDepartmentUserRole;
    }

    public async Task<List<DepartmentData>> GetDepartmentsAsync(
      List<Guid> departmentsIds = null,
      List<Guid> usersIds = null,
      List<string> errors = null)
    {
      List<Guid> allGuids = new();

      if (usersIds is not null)
      {
        allGuids.AddRange(usersIds);
      }

      if (departmentsIds is not null)
      {
        allGuids.AddRange(departmentsIds);
      }

      object request = IGetDepartmentsRequest.CreateObj(departmentsIds: departmentsIds, usersIds: usersIds);

      List<DepartmentData> departments = await _globalCache
        .GetAsync<List<DepartmentData>>(Cache.Departments, allGuids.GetRedisCacheKey(
          nameof(IGetDepartmentsRequest), request.GetBasicProperties()));

      if (departments is not null)
      {
        _logger.LogInformation(
          $"Department was taken from the cache.");
      }
      else
      {
        departments = (await _rcGetDepartments.ProcessRequest<IGetDepartmentsRequest, IGetDepartmentsResponse>(
            request,
            errors,
            _logger))
          ?.Departments;
      }

      return departments;
    }

    public async Task<DepartmentUserRole?> GetDepartmentUserRoleAsync(Guid departmentId, Guid userId, List<string> errors = null)
    {
      IGetDepartmentUserRoleResponse response = await _rcGetDepartmentUserRole.ProcessRequest<IGetDepartmentUserRoleRequest, IGetDepartmentUserRoleResponse>(
        IGetDepartmentUserRoleRequest.CreateObj(
          departmentId: departmentId,
          userId: userId),
        errors,
        _logger);

      return response?.DepartmentUserRole;
    }
  }
}
