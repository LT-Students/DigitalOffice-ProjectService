using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Broker.Requests;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetProjectInfoConsumer : IConsumer<IGetProjectRequest>
    {
        private readonly IProjectRepository _repository;

        private object GetProjectIds(Guid projectId)
        {
            var dbProject = _repository.GetProject(projectId);

            if (dbProject == null)
            {
                throw new NotFoundException($"Project with id: {projectId} was not found.");
            }

            return IGetProjectRequest.CreateObj(dbProject.Id);
        }

        public GetProjectInfoConsumer(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetProjectRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetProjectIds, context.Message.Id);

            await context.RespondAsync<IOperationResult<IProjectResponse>>(response);
        }
    }
}
