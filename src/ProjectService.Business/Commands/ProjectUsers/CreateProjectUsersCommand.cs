using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class CreateProjectUsersCommand : ICreateProjectUsersCommand
  {
    private readonly IUserRepository _repository;
    private readonly IDbProjectUserMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IAddUsersToProjectValidator _validator;
    private readonly ILogger<CreateProjectUsersCommand> _logger;
    private readonly IRequestClient<ICreateWorkTimeRequest> _rcCreateWorkTime;
    private readonly IResponseCreater _responseCreater;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheNotebook _cacheNotebook;

    private async Task CreateWorkTimeAsync(Guid projectId, List<Guid> usersIds, List<string> errors)
    {
      const string logMessage = "Failed to create a work time for project {projectId} with users {userIds}";

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcCreateWorkTime.GetResponse<IOperationResult<bool>>(
            ICreateWorkTimeRequest.CreateObj(projectId, usersIds));

        if (response.Message.IsSuccess && response.Message.Body)
        {
          return;
        }

        _logger.LogWarning(logMessage, projectId, string.Join(", ", usersIds));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, projectId, string.Join(", ", usersIds));
      }

      errors.Add($"Failed to create a work time for project {projectId} with users: {string.Join(", ", usersIds)}.");
    }

    public CreateProjectUsersCommand(
      IUserRepository repository,
      IDbProjectUserMapper mapper,
      IAccessValidator accessValidator,
      IAddUsersToProjectValidator validator,
      ILogger<CreateProjectUsersCommand> logger,
      IRequestClient<ICreateWorkTimeRequest> rcCreateWorkTime,
      IResponseCreater responseCreater,
      IHttpContextAccessor httpContextAccessor,
      ICacheNotebook cacheNotebook)
    {
      _mapper = mapper;
      _validator = validator;
      _repository = repository;
      _accessValidator = accessValidator;
      _logger = logger;
      _rcCreateWorkTime = rcCreateWorkTime;
      _responseCreater = responseCreater;
      _httpContextAccessor = httpContextAccessor;
      _cacheNotebook = cacheNotebook;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(CreateProjectUsersRequest request)
    {
      List<string> errors = new();

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _repository.IsProjectAdminAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId()))
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

      if (!validationResult.IsValid)
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      List<Guid> existUsers = await _repository.GetExistAsync(request.ProjectId, request.Users.Select(u => u.UserId).ToList());

      request.Users = request.Users.GroupBy(u => u.UserId).Select(g => g.First()).Where(u => !existUsers.Contains(u.UserId)).ToList();

      if (!request.Users.Any())
      {
        errors.Add("Request doesn't contains users who still are not emloyees of this project.");

        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      bool result = await _repository.CreateAsync(_mapper.Map(request));

      await CreateWorkTimeAsync(request.ProjectId, request.Users.Select(u => u.UserId).ToList(), errors);

      if (existUsers.Any())
      {
        errors.Add("Exist user were not added again.");
      }

      if (result)
      {
        await _cacheNotebook.RemoveAsync(request.ProjectId);
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
