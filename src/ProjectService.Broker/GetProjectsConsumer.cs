using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class GetProjectsConsumer : IConsumer<IGetProjectsRequest>
  {
    private readonly IProjectRepository _projectRepository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly ICacheNotebook _cacheNotebook;
    private readonly IRedisHelper _redisHelper;

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
          p.Users?.Select(
            u => new ProjectUserData(
              u.UserId,
              u.ProjectId,
              u.CreatedAtUtc))
            .ToList()))
          .ToList(), totalCount);
    }

    private string CreateKey(IGetProjectsRequest request)
    {
      List<Guid> ids = new();

      if (request.ProjectsIds != null && request.ProjectsIds.Any())
      {
        ids.AddRange(request.ProjectsIds);
      }

      if (request.UserId.HasValue)
      {
        ids.Add(request.UserId.Value);
      }

      if (request.DepartmentId.HasValue)
      {
        ids.Add(request.DepartmentId.Value);
      }

      List<object> additionalArguments = new() { request.IncludeUsers };

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
      IOptions<RedisConfig> redisConfig,
      ICacheNotebook cacheNotebook,
      IRedisHelper redisHelper)
    {
      _projectRepository = projectRepository;
      _redisConfig = redisConfig;
      _cacheNotebook = cacheNotebook;
      _redisHelper = redisHelper;
    }

    public async Task Consume(ConsumeContext<IGetProjectsRequest> context)
    {
      (List<ProjectData> projects, int totalCount) = await GetProjectsAsync(context.Message);

      object response = OperationResultWrapper.CreateResponse((_) => IGetProjectsResponse.CreateObj(projects, totalCount), context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectsResponse>>(response);

      if (projects != null && projects.Any())
      {
        string key = CreateKey(context.Message);

        await _redisHelper.CreateAsync(
          Cache.Projects,
          key,
          (projects, totalCount),
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));

        _cacheNotebook.Add(projects.Select(p => p.Id).ToList(), Cache.Projects, key);
      }
    }
  }
}
