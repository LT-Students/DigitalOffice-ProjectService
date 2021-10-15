using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces
{
  [AutoInject]
  public interface IRemoveUsersFromProjectCommand
  {
    OperationResultResponse<bool> Execute(Guid projectId, List<Guid> userIds);
  }
}
