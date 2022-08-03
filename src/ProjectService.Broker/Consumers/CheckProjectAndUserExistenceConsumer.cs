using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class CheckProjectAndUserExistenceConsumer : IConsumer<ICheckProjectAndUserExistenceRequest>
  {
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IProjectRepository _projectRepository;

    private async Task<object> CheckProjectAndUserExistenceAsync(ICheckProjectAndUserExistenceRequest request)
    {
      bool? isManager = null;
      bool isProjectExists = await _projectRepository.DoesExistAsync(request.ProjectId);

      if (request.UserId is not null && isProjectExists)
      {
        isManager = await _projectUserRepository.DoesExistAsync(request.UserId.Value, request.ProjectId, true);
      }

      return ICheckProjectAndUserExistenceResponse.CreateObj(
        isProjectExists: isProjectExists,
        isUserManager: isManager);
    }

    public CheckProjectAndUserExistenceConsumer(
      IProjectUserRepository projectUserRepository,
      IProjectRepository projectRepository)
    {
      _projectUserRepository = projectUserRepository;
      _projectRepository = projectRepository;
    }

    public async Task Consume(ConsumeContext<ICheckProjectAndUserExistenceRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(CheckProjectAndUserExistenceAsync, context.Message);

      await context.RespondAsync<IOperationResult<ICheckProjectAndUserExistenceResponse>>(response);
    }
  }
}
