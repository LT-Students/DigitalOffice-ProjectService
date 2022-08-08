using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class GetProjectUserRoleConsumer : IConsumer<IGetProjectUserRoleRequest>
  {
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IProjectRepository _projectRepository;

    private async Task<object> CheckProjectAndUserExistenceAsync(IGetProjectUserRoleRequest request)
    {
      DbProject project = await _projectRepository.GetProjectWithUserAsync(request.ProjectId, request.UserId);

      DbProjectUser user = project?.Users.Where(x => x.Id == request.UserId).FirstOrDefault();

      ProjectUserRoleType? userRole = ProjectUserRoleType.Observer;
      if (project is null)
      {
        userRole = null;
      } 
      else if (user is not null)
      {
        userRole = (ProjectUserRoleType?)user.Role;
      }

      return IGetProjectUserRoleResponse.CreateObj(
        projectStatus: project is null
          ? ProjectStatusType.DoesNotExist
          : (ProjectStatusType)project.Status,
        projectUserRole: userRole);
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
