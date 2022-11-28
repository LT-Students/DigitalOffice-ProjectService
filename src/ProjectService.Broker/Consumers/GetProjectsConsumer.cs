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

    private async Task<List<ProjectData>> GetProjectsAsync(IGetProjectsRequest request)
    {
      List<DbProject> projects = await _projectRepository.GetAsync(request);

      return projects.Select(
        p => new ProjectData(
          id: p.Id,
          name: p.Name,
          status: ((ProjectStatusType)p.Status).ToString(),
          shortName: p.ShortName,
          shortDescription: p.ShortDescription,
          department: _projectDepartmentDataMapper.Map(p.Department),
          users: p.Users?.Select(u => new ProjectUserData(
            userId: u.UserId,
            projectId: u.ProjectId,
            isActive: u.IsActive,
            projectUserRole: (ProjectUserRoleType)u.Role)).ToList()))
        .ToList();
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
      List<ProjectData> projects = await GetProjectsAsync(context.Message);

      object response = OperationResultWrapper.CreateResponse(_ => IGetProjectsResponse.CreateObj(projects), context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectsResponse>>(response);

      if (projects is not null && projects.Any())
      {
        List<Guid> allGuids = new();

        if (context.Message.ProjectsIds is not null)
        {
          allGuids.AddRange(context.Message.ProjectsIds);
        }

        if (context.Message.UsersIds is not null)
        {
          allGuids.AddRange(context.Message.UsersIds);
        }

        if (context.Message.DepartmentsIds is not null)
        {
          allGuids.AddRange(context.Message.DepartmentsIds);
        }

        if (allGuids.Any())
        {
          await _globalCache.CreateAsync(
            database: Cache.Projects,
            key: allGuids.GetRedisCacheKey(nameof(IGetProjectsRequest), context.Message.GetBasicProperties()),
            item: projects,
            elementsIds: allGuids,
            lifeTime: TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
        }
      }
    }
  }
}
