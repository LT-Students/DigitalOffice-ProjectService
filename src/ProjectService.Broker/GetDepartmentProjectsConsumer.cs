using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetDepartmentProjectsConsumer : IConsumer<IGetDepartmentProjectsRequest>
    {
        private readonly IProjectRepository _repository;

        private object GetProjectIds(Guid departmentId)
        {
            return IGetDepartmentProjectsResponse.CreateObj(
                _repository.Get(departmentId)
                    .Select(p =>
                        new ProjectData(
                            p.Id,
                            p.Name,
                            ((ProjectStatusType)p.Status).ToString(),
                            p.ShortName,
                            p.Description,
                            p.ShortDescription))
                    .ToList());
        }

        public GetDepartmentProjectsConsumer(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetDepartmentProjectsRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetProjectIds, context.Message.DepartmentId);

            await context.RespondAsync<IOperationResult<IGetDepartmentProjectsResponse>>(response);
        }
    }
}
