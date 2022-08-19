using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces
{
  [AutoInject]
  public interface IFindProjectUsersCommand
  {
    Task<FindResultResponse<UserInfo>> ExecuteAsync(Guid projectId, FindProjectUsersFilter filter);
  }
}
