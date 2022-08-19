using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class DisactivateProjectUserConsumer : IConsumer<IDisactivateUserPublish>
  {
    private readonly IProjectUserRepository _repository;
    private readonly IGlobalCacheRepository _globalCache;

    public DisactivateProjectUserConsumer(
      IProjectUserRepository repository,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserPublish> context)
    {
      List<Guid> projectsIds = await _repository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);

      if (projectsIds.Any())
      {
        projectsIds.ForEach(async projectId => await _globalCache.RemoveAsync(projectId));
      }
    }
  }
}
