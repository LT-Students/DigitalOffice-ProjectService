using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class GetProjectsConsumer : IConsumer<IGetProjectsRequest>
  {
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectDepartmentDataMapper _projectDepartmentDataMapper;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<(List<ProjectData> projects, int totalCount)> GetProjectsAsync(IGetProjectsRequest request)
    {
      (List<DbProject> projects, int totalCount) = (await _projectRepository.GetAsync(request));

      return (projects.Select(
        p => new ProjectData(
          p.Id,
          p.Name,
          ((ProjectStatusType)p.Status).ToString(),
          p.ShortName,
          p.ShortDescription,
          _projectDepartmentDataMapper.Map(p.Department),
          p.Users?.Select(
            u => new ProjectUserData(
              u.UserId,
              u.ProjectId,
              u.IsActive,
              (ProjectUserRoleType)u.Role))
            .ToList()))
          .ToList(), totalCount);
    }

    private string CreateKey(IGetProjectsRequest request)
    {
      List<Guid> ids = new();

      if (request.ProjectsIds is not null && request.ProjectsIds.Any())
      {
        ids.AddRange(request.ProjectsIds);
      }

      if (request.UsersIds is not null && request.UsersIds.Any())
      {
        ids.AddRange(request.UsersIds);
      }

      if (request.DepartmentsIds is not null && request.DepartmentsIds.Any())
      {
        ids.AddRange(request.DepartmentsIds);
      }

      List<object> additionalArguments = new() { request.IncludeDepartment, request.IncludeUsers };

      if (request.AscendingSort.HasValue)
      {
        additionalArguments.Add(request.AscendingSort.Value);
      }

      if (request.SkipCount.HasValue)
      {
        additionalArguments.Add(request.SkipCount.Value);
      }

      if (request.TakeCount.HasValue)
      {
        additionalArguments.Add(request.TakeCount.Value);
      }

      return ids.GetRedisCacheHashCode(additionalArguments.ToArray());
    }

    public GetProjectsConsumer(
      IProjectRepository projectRepository,
      IProjectDepartmentDataMapper projectDepartmentDataMapper,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _projectRepository = projectRepository;
      _projectDepartmentDataMapper = projectDepartmentDataMapper;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IGetProjectsRequest> context)
    {
      (List<ProjectData> projects, int totalCount) = await GetProjectsAsync(context.Message);

      object response = OperationResultWrapper.CreateResponse((_) => IGetProjectsResponse.CreateObj(projects, totalCount), context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectsResponse>>(response);

      if (projects != null && projects.Any())
      {
        string key = CreateKey(context.Message);

        await _globalCache.CreateAsync(
          Cache.Projects,
          key,
          (projects, totalCount),
          projects.Select(p => p.Id).ToList(),
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}
