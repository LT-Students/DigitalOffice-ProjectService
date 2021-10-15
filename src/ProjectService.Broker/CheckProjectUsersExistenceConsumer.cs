using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class CheckProjectUsersExistenceConsumer : IConsumer<ICheckProjectUsersExistenceRequest>
  {
    private readonly IUserRepository _userRepository;

    public CheckProjectUsersExistenceConsumer(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<ICheckProjectUsersExistenceRequest> context)
    {
      List<Guid> existUsers = await _userRepository.DoExistAsync(context.Message.ProjectId, context.Message.UsersIds);

      object response = OperationResultWrapper.CreateResponse((_) => existUsers, context);

      await context.RespondAsync<IOperationResult<List<Guid>>>(response);

      // todo cache
    }
  }
}
