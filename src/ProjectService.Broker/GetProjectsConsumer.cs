using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class GetProjectsConsumer : IConsumer<IGetProjectsRequest>
  {
    private readonly IProjectRepository _projectRepository;
    private readonly IConnectionMultiplexer _cache;
    private readonly IOptions<RedisConfig> _redisConfig;

    private (List<ProjectData> projects, int totalCount) GetProjects(IGetProjectsRequest request)
    {
      return (_projectRepository.Get(request, out int totalCount)
        .Select(
          p => new ProjectData(
            p.Id,
            p.DepartmentId,
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

      return ids.GetRedisCacheHashCode(additionalArguments);
    }

    public GetProjectsConsumer(
      IProjectRepository projectRepository,
      IConnectionMultiplexer cache,
      IOptions<RedisConfig> redisConfig)
    {
      _projectRepository = projectRepository;
      _cache = cache;
      _redisConfig = redisConfig;
    }

    public async Task Consume(ConsumeContext<IGetProjectsRequest> context)
    {
      (List<ProjectData> projects, int totalCount) = GetProjects(context.Message);

      object response = OperationResultWrapper.CreateResponse((_) => IGetProjectsResponse.CreateObj(projects, totalCount), context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectsResponse>>(response);

      await _cache.GetDatabase(Cache.Projects).StringSetAsync(CreateKey(context.Message),
        JsonConvert.SerializeObject((projects, totalCount)),
        TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
    }
  }
}
