using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
    [AutoInject]
    public interface IGetProjectCommand
    {
        OperationResultResponse<ProjectResponse> Execute(GetProjectFilter filter);
    }
}