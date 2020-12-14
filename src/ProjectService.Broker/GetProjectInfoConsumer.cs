using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    /// <summary>
    /// Consumer for getting information about the user on project.
    /// </summary>
    public class GetProjectInfoConsumer : IConsumer<IGetProjectUserRequest>
    {
        private readonly IProjectRepository repository;

        public GetProjectInfoConsumer(
            [FromServices] IProjectRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetProjectUserRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetProjectInfo, context.Message);

            await context.RespondAsync<IOperationResult<IGetProjectUserResponse>>(response);
        }

        private object GetProjectInfo(IGetProjectUserRequest request)
        {
            var dbProjectUser = repository.GetProjectUsers(request.ProjectId, true)
                .FirstOrDefault(x => x.UserId == request.UserId);

            if (dbProjectUser == null)
            {
                throw new NotFoundException();
            }

            return IGetProjectUserResponse.CreateObj(dbProjectUser.Id, dbProjectUser.IsActive);
        }
    }
}
