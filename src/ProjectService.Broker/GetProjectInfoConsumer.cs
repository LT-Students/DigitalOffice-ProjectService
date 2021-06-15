using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetProjectInfoConsumer : IConsumer<IGetProjectRequest>
    {
        private readonly IProjectRepository _repository;

        private object GetProjectInfo(Guid projectId)
        {
            var filter = new GetProjectFilter { ProjectId = projectId };
            var dbProject = _repository.GetProject(filter);

            if (dbProject == null)
            {
                throw new NotFoundException($"Project with id: {projectId} was not found.");
            }

            return IProjectResponse.CreateObj(dbProject.Id, dbProject.Name, (int)dbProject.Status);
        }

        public GetProjectInfoConsumer(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetProjectRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetProjectInfo, context.Message.ProjectId);

            await context.RespondAsync<IOperationResult<IProjectResponse>>(response);
        }
    }
}
