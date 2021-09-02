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
    public class GetProjectsConsumer : IConsumer<IGetProjectsRequest>
    {
        private readonly IProjectRepository _projectRepository;

        private object GetProjects(IGetProjectsRequest request)
        {
            return IGetProjectsResponse.CreateObj(
                _projectRepository.Get(request, out int totalCount)
                    .Select(
                        p => new ProjectData(
                            p.Id,
                            p.DepartmentId,
                            p.Name,
                            ((ProjectStatusType)p.Status).ToString(),
                            p.ShortName,
                            p.Description,
                            p.ShortDescription,
                            p.Users?.Select(
                                u => new ProjectUserData(
                                    u.UserId,
                                    u.ProjectId,
                                    u.CreatedAtUtc))
                                .ToList()))
                    .ToList(),
                totalCount);
        }

        public GetProjectsConsumer(
            IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task Consume(ConsumeContext<IGetProjectsRequest> context)
        {
            object response = OperationResultWrapper.CreateResponse(GetProjects, context.Message);

            await context.RespondAsync<IOperationResult<IGetProjectsResponse>>(response);
        }
    }
}
