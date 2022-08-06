using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class GetProjectUserRoleConsumer : IConsumer<IGetProjectUserRoleRequest>
  {
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IProjectRepository _projectRepository;

    private async Task<object> CheckProjectAndUserExistenceAsync(IGetProjectUserRoleRequest request)
    {
      ProjectUserRoleType? projectUserRole = null;
      ProjectStatusType projectType = await _projectRepository.GetProjectStatusAsync(request.ProjectId);

      if (projectType.Equals(ProjectStatusType.Active))
      {
        projectUserRole = await _projectUserRepository.GetUserRoleAsync(request.ProjectId, request.UserId);
      }

      return IGetProjectUserRoleResponse.CreateObj(
        projectStatus: projectType,
        projectUserRole: projectUserRole);
    }

    public GetProjectUserRoleConsumer(
      IProjectUserRepository projectUserRepository,
      IProjectRepository projectRepository)
    {
      _projectUserRepository = projectUserRepository;
      _projectRepository = projectRepository;
    }

    public async Task Consume(ConsumeContext<IGetProjectUserRoleRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(CheckProjectAndUserExistenceAsync, context.Message);

      await context.RespondAsync<IOperationResult<IGetProjectUserRoleResponse>>(response);
    }
  }
}
