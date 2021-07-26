using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class FindProjectsConsumer : IConsumer<IFindProjectsRequest>
    {
        private readonly IProjectRepository _repository;

        private object FindProjects(IFindProjectsRequest request)
        {
            return IFindProjectsResponse.CreateObj(_repository.Find(request.ProjectIds)
                .Select(p => new ProjectData(
                    p.Id,
                    p.Name,
                    ((ProjectStatusType)p.Status).ToString(),
                    p.ShortName,
                    p.Description,
                    p.ShortDescription)).ToList());
        }

        public FindProjectsConsumer(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IFindProjectsRequest> context)
        {
            object response = OperationResultWrapper.CreateResponse(FindProjects, context.Message);

            await context.RespondAsync<IOperationResult<IFindProjectsResponse>>(response);
        }
    }
}
