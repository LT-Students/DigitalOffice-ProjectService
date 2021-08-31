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
    public class FindProjectUsersConsumer : IConsumer<IFindProjectUsersRequest>
    {
        private readonly IUserRepository _userRepository;

        private object FindUsers(IFindProjectUsersRequest request)
        {
            return IFindProjectUsersResponse.CreateObj(
                _userRepository.Find(request.ProjectId, request.SkipCount, request.TakeCount, out int totalCount)
                .Select(
                    pu => new ProjectUserData(pu.UserId, pu.ProjectId, pu.CreatedAtUtc))
                .ToList(),
                totalCount);
        }

        public FindProjectUsersConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<IFindProjectUsersRequest> context)
        {
            object response = OperationResultWrapper.CreateResponse(FindUsers, context.Message);

            await context.RespondAsync<IOperationResult<IFindProjectUsersResponse>>(response);
        }
    }
}
