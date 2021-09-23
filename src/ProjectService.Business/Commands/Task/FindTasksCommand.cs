using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
  public class FindTasksCommand : IFindTasksCommand
  {
    private readonly ITaskInfoMapper _mapper;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IBaseFindRequestValidator _findRequestValidator;
    private readonly ILogger<FindTasksCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetUsersDataRequest> _requestClient;

    private IGetUsersDataResponse GetUsersData(List<Guid> userId, List<string> errors)
    {
      string errorMessage = "Can not find user data. Please try again later.";

      try
      {
        Response<IOperationResult<IGetUsersDataResponse>> response = _requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
          IGetUsersDataRequest.CreateObj(userId)).Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }

        errors.AddRange(response.Message.Errors);

        _logger.LogWarning("Can not find user data with this id {UserId}: " +
          $"{Environment.NewLine}{string.Join('\n', response.Message.Errors)}", userId);
      }
      catch (Exception exc)
      {
        errors.Add(errorMessage);

        _logger.LogError(exc, errorMessage);
      }

      return null;
    }

    public FindTasksCommand(
      ITaskInfoMapper mapper,
      ITaskRepository taskRepository,
      IUserRepository userRepository,
      ILogger<FindTasksCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IGetUsersDataRequest> requestClient,
      IBaseFindRequestValidator findRequestValidator)
    {
      _mapper = mapper;
      _logger = logger;
      _requestClient = requestClient;
      _taskRepository = taskRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _findRequestValidator = findRequestValidator;
    }

    public FindResultResponse<TaskInfo> Execute(FindTasksFilter filter)
    {
      FindResultResponse<TaskInfo> response = new();

      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      List<DbProjectUser> projectUsers = _userRepository.Find(userId).ToList();

      if (!(projectUsers.Any() || _accessValidator.IsAdmin()))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Concat(new List<string> { "Not enough rights." });

        return response;
      }

      if (_findRequestValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Errors.Concat(errors);

        return response;
      }

      IEnumerable<Guid> projectIds = projectUsers.Select(x => x.ProjectId);
      List<DbTask> dbTasks = _taskRepository.Find(filter, projectIds, out int totalCount).ToList();

      List<Guid> users = dbTasks.Where(x => x.AssignedTo.HasValue).Select(x => x.AssignedTo.Value).ToList();
      users.AddRange(dbTasks.Select(x => x.CreatedBy).ToList());

      IGetUsersDataResponse usersData = GetUsersData(users, response.Errors.ToList());

      List<TaskInfo> tasks = new();
      foreach (DbTask dbTask in dbTasks)
      {
        UserData assignedUser = usersData?.UsersData.FirstOrDefault(x => x.Id == dbTask.AssignedTo);
        UserData author = usersData?.UsersData.FirstOrDefault(x => x.Id == dbTask.CreatedBy);

        tasks.Add(_mapper.Map(dbTask, assignedUser, author));
      }

      response.TotalCount = totalCount;
      response.Body = tasks;

      return response;
    }
  }
}
