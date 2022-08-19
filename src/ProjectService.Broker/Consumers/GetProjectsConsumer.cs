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

      return ids.GetRedisCacheHashCode(request.IncludeDepartment, request.IncludeUsers);
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

      if (projects != null && projects.Any())
      {
        string key = CreateKey(context.Message);

        await _globalCache.CreateAsync(
          database: Cache.Projects,
          key: key,
          item: projects,
          elementsIds: projects.Select(p => p.Id).ToList(),
          lifeTime: TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}
