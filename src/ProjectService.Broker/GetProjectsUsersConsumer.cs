using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetProjectsUsersConsumer : IConsumer<IGetProjectsUsersRequest>
    {
        private readonly IProjectRepository _projectRepository;

        private object GetProjectUsers(object arg)
        {
            return IGetProjectsUsersResponse.CreateObj(_projectRepository.GetProjectsUsers());
        }

        public GetProjectsUsersConsumer(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task Consume(ConsumeContext<IGetProjectsUsersRequest> context)
        {
            object response = OperationResultWrapper.CreateResponse(GetProjectUsers, context);

            await context.RespondAsync<IOperationResult<IGetProjectsUsersResponse>>(response);
        }
    }
}
