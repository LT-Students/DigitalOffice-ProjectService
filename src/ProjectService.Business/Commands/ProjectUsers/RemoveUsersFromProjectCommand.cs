using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class RemoveUsersFromProjectCommand : IRemoveUsersFromProjectCommand
  {
    private readonly IUserRepository _repository;
    private readonly IAccessValidator _accessValidator;

    public RemoveUsersFromProjectCommand(
      IUserRepository repository,
      IAccessValidator accessValidator)
    {
      _repository = repository;
      _accessValidator = accessValidator;
    }

    // TODO make async

    public OperationResultResponse<bool> Execute(Guid projectId, List<Guid> userIds)
    {
      if (userIds == null || !userIds.Any())
      {
        throw new BadRequestException("Users not specified.");
      }

      if (!(_accessValidator.HasRights(Rights.AddEditRemoveProjects)))
      {
        throw new ForbiddenException("Not enough rights.");
      }

      bool result = _repository.DisableWorkersInProject(projectId, userIds);

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
