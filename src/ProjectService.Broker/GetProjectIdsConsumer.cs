using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Broker.Requests;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Exceptions.Models;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetProjectIdsConsumer : IConsumer<IGetUserProjectsRequest>
    {
        private readonly IUserRepository _repository;

        private object GetProjectIds(Guid userId)
        {
            var dbUsers = _repository.Get(userId);

            if (dbUsers == null)
            {
                throw new NotFoundException($"User with id: {userId} was not found.");
            }

            var projectIds = dbUsers.Select(x => x.ProjectId).ToList();

            return IProjectsResponse.CreateObj(projectIds);
        }

        public GetProjectIdsConsumer(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetUserProjectsRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetProjectIds, context.Message.UserId);

            await context.RespondAsync<IOperationResult<IProjectsResponse>>(response);
        }
    }
}
