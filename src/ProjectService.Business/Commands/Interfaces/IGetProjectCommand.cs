using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    [AutoInject]
    public interface IGetProjectCommand
    {
        ProjectResponse Execute(GetProjectFilter filter);
    }
}