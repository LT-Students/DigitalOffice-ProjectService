using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetProjectsUsersConsumer : IConsumer<IGetProjectsUsersRequest>
    {
        private readonly IUserRepository _userRepository;

        private object GetProjectUsers(IGetProjectsUsersRequest request)
        {
            return IGetProjectsUsersResponse.CreateObj(
                _userRepository.Get(request, out int totalCount)
                .Select(
                    p =>
                        new ProjectUserData(
                            p.UserId,
                            p.ProjectId,
                            p.CreatedAtUtc
                        ))
                .ToList(),
                totalCount);
        }

        public GetProjectsUsersConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<IGetProjectsUsersRequest> context)
        {
            object response = OperationResultWrapper.CreateResponse(GetProjectUsers, context.Message);

            await context.RespondAsync<IOperationResult<IGetProjectsUsersResponse>>(response);
        }
    }
}
