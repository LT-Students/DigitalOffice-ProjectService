using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Department;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Department.Interfaces
{
  [AutoInject]
  public interface IEditProjectDepartmentCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(EditProjectDepartmentRequest request);
  }
}
