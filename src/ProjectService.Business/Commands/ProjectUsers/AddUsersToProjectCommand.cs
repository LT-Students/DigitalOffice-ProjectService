using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class AddUsersToProjectCommand : IAddUsersToProjectCommand
  {
    private readonly IUserRepository _repository;
    private readonly IDbProjectUserMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IAddUsersToProjectValidator _validator;
    private readonly ILogger<AddUsersToProjectCommand> _logger;
    private readonly IRequestClient<ICreateWorkTimeRequest> _rcCreateWorkTime;

    // TODO make async
    private void CreateWorkTime(Guid projectId, List<Guid> userIds, List<string> errors)
    {
      string errorMessage = $"Failed to create a work time for project {projectId} with users: {string.Join(", ", userIds)}.";
      const string logMessage = "Failed to create a work time for project {projectId} with users {userIds}";

      try
      {
        var response = _rcCreateWorkTime.GetResponse<IOperationResult<bool>>(
            ICreateWorkTimeRequest.CreateObj(projectId, userIds)).Result;

        if (!(response.Message.IsSuccess && response.Message.Body))
        {
          _logger.LogWarning(logMessage, projectId, string.Join(", ", userIds));
          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, projectId, string.Join(", ", userIds));

        errors.Add(errorMessage);
      }
    }

    public AddUsersToProjectCommand(
      IUserRepository repository,
      IDbProjectUserMapper mapper,
      IAccessValidator accessValidator,
      IAddUsersToProjectValidator validator,
      ILogger<AddUsersToProjectCommand> logger,
      IRequestClient<ICreateWorkTimeRequest> rcCreateWorkTime)
    {
      _mapper = mapper;
      _validator = validator;
      _repository = repository;
      _accessValidator = accessValidator;
      _logger = logger;
      _rcCreateWorkTime = rcCreateWorkTime;
    }

    //Todo rework this
    public OperationResultResponse<bool> Execute(AddUsersToProjectRequest request)
    {
      List<string> errors = new();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveProjects))
      {
        throw new ForbiddenException("Not enough rights");
      }

      _validator.ValidateAndThrowCustom(request);

      List<Guid> existUsers = _repository.GetExistProjectUsers(request.ProjectId, request.Users.Select(u => u.UserId).ToList());

      request.Users = request.Users.GroupBy(u => u.UserId).Select(g => g.First()).Where(u => !existUsers.Contains(u.UserId)).ToList();

      if (!request.Users.Any())
      {
        throw new BadRequestException("Request doesn't contain any new users.");
      }

      bool result = _repository.AddUsersToProject(_mapper.Map(request));

      CreateWorkTime(request.ProjectId, request.Users.Select(u => u.UserId).ToList(), errors);

      if (existUsers.Any())
      {
        errors.Add("Exist user were not added again.");
      }

      return new()
      {
        Status = errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess,
        Body = result,
        Errors = errors
      };
    }
  }
}
