using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class GetProjectsUsersConsumer : IConsumer<IGetProjectsUsersRequest>
  {
    private readonly IUserRepository _userRepository;

    private async Task<object> GetProjectUsersAsync(IGetProjectsUsersRequest request)
    {
      (List<DbProjectUser> users, int totalCount) = await _userRepository.GetAsync(request);


      return IGetProjectsUsersResponse.CreateObj(users.Select(
        p =>
          new ProjectUserData(
            p.UserId,
            p.ProjectId,
            p.CreatedAtUtc
          )).ToList(),
        totalCount);
    }

    public GetProjectsUsersConsumer(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<IGetProjectsUsersRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(GetProjectUsersAsync, context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectsUsersResponse>>(response);
    }
  }
}
