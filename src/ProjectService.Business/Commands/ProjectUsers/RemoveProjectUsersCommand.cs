using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class RemoveProjectUsersCommand : IRemoveProjectUsersCommand
  {
    private readonly IUserRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreater;

    public RemoveProjectUsersCommand(
      IUserRepository repository,
      IAccessValidator accessValidator,
      IResponseCreater responseCreater)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _responseCreater = responseCreater;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid projectId, List<Guid> userIds)
    {
      if (!(await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)))
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (userIds == null || !userIds.Any())
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      bool result = await _repository.RemoveAsync(projectId, userIds);

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
