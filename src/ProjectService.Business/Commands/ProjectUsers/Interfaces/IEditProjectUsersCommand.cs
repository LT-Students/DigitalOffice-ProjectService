using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces
{
  [AutoInject]
  public interface IEditProjectUsersCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(EditProjectUsersRequest request);
  }
}
