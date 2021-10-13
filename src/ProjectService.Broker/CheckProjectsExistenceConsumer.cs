using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class CheckProjectsExistenceConsumer : IConsumer<ICheckProjectsExistence>
  {
    private readonly IProjectRepository _projectRepository;
    private readonly IRedisHelper _redisHelper;

    public CheckProjectsExistenceConsumer(
      IProjectRepository projectRepository,
      IRedisHelper redisHelper)
    {
      _projectRepository = projectRepository;
      _redisHelper = redisHelper;
    }

    public async Task Consume(ConsumeContext<ICheckProjectsExistence> context)
    {
      List<Guid> existProjects = _projectRepository.DoExist(context.Message.ProjectsIds);

      object response = OperationResultWrapper.CreateResponse((_) => ICheckProjectsExistence.CreateObj(existProjects), context);

      await context.RespondAsync<IOperationResult<ICheckProjectsExistence>>(response);

      // todo add cache
    }
  }
}
