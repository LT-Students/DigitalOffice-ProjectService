using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
  [AutoInject]
    public interface IGetProjectCommand
    {
        Task<OperationResultResponse<ProjectResponse>> ExecuteAsync(GetProjectFilter filter);
    }
}
