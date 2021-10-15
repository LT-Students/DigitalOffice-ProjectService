using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces
{
  [AutoInject]
  public interface IRemoveUsersFromProjectCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid projectId, List<Guid> userIds);
  }
}
