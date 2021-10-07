using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
  public interface IGetProjectCommand
  {
    Task<OperationResultResponse<ProjectResponse>> Execute(GetProjectFilter filter);
  }
}
