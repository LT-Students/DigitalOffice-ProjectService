using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class GetProjectsUsersConsumer : IConsumer<IGetProjectsUsersRequest>
  {
    private readonly IProjectUserRepository _projectUserRepository;

    private async Task<object> GetProjectUsersAsync(IGetProjectsUsersRequest request)
    {
      (List<DbProjectUser> users, int totalCount) = await _projectUserRepository.GetAsync(request);

      return IGetProjectsUsersResponse.CreateObj(users.Select(
        p =>
          new ProjectUserData(
            p.UserId,
            p.ProjectId,
            p.IsActive,
            (ProjectUserRoleType)p.Role
          )).ToList(),
        totalCount);
    }

    public GetProjectsUsersConsumer(IProjectUserRepository projectUserRepository)
    {
      _projectUserRepository = projectUserRepository;
    }

    public async Task Consume(ConsumeContext<IGetProjectsUsersRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(GetProjectUsersAsync, context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectsUsersResponse>>(response);
    }
  }
}
