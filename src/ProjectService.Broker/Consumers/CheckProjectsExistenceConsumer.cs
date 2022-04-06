using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class CheckProjectsExistenceConsumer : IConsumer<ICheckProjectsExistence>
  {
    private readonly IProjectRepository _projectRepository;

    public CheckProjectsExistenceConsumer(
      IProjectRepository projectRepository)
    {
      _projectRepository = projectRepository;
    }

    public async Task Consume(ConsumeContext<ICheckProjectsExistence> context)
    {
      List<Guid> existProjects = await _projectRepository.DoExistAsync(context.Message.ProjectsIds);

      object response = OperationResultWrapper.CreateResponse((_) => ICheckProjectsExistence.CreateObj(existProjects), context);

      await context.RespondAsync<IOperationResult<ICheckProjectsExistence>>(response);

      // todo add cache
    }
  }
}
