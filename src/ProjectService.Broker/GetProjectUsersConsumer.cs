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
    public class GetProjectUsersConsumer : IConsumer<IGetProjectUsersRequest>
    {
        private readonly IUserRepository _userRepository;

        private object GetProjectUsers(IGetProjectUsersRequest request)
        {
            var users = _userRepository.Find(request.ProjectUsers.Select(pu => pu.userId).ToList());

            return IGetProjectUsersResponse.CreateObj(
                users
                    .Where(dbpu => request.ProjectUsers.Any(pu => pu.projectId == dbpu.ProjectId && pu.userId == dbpu.UserId))
                    .Select(pu => new ProjectUserData(pu.UserId, pu.ProjectId, pu.AddedOn))
                    .ToList());
        }

        public GetProjectUsersConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<IGetProjectUsersRequest> context)
        {
            object result = OperationResultWrapper.CreateResponse(GetProjectUsers, context.Message);

            await context.RespondAsync<IOperationResult<IGetProjectUsersResponse>>(result);
        }
    }
}
