using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class DisactivateProjectUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IUserRepository _repository;
    private readonly IGlobalCacheRepository _globalCache;

    public DisactivateProjectUserConsumer(
      IUserRepository repository,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await _repository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);

      List<DbProjectUser> projectsUsers = await _repository.GetAsync(context.Message.UserId);
      projectsUsers.ForEach(async pj => await _globalCache.RemoveAsync(pj.ProjectId));
    }
  }
}
