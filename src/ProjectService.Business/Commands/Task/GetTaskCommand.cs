using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task
{
  public class GetTaskCommand : IGetTaskCommand
  {
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly HttpContext _httpContext;
    private readonly ITaskResponseMapper _taskResponseMapper;
    private readonly ITaskInfoMapper _taskInfoMapper;
    private readonly ILogger<GetTaskCommand> _logger;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployee;
    private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
    private readonly IConnectionMultiplexer _cache;

    private async Task<DepartmentData> GetDepartment(Guid authorId, List<string> errors)
    {
      RedisValue departmentFromCache = await _cache.GetDatabase(Cache.Departments).StringGetAsync(authorId.GetRedisCacheHashCode());

      if (departmentFromCache.HasValue)
      {
        return JsonConvert.DeserializeObject<List<DepartmentData>>(departmentFromCache).FirstOrDefault();
      }

      return await GetDepartmentThroughBroker(authorId, errors);
    }

    private async Task<DepartmentData> GetDepartmentThroughBroker(Guid authorId, List<string> errors)
    {
      string errorMessage = "Cannot get department. Please try again later.";

      try
      {
        var response = await _rcGetCompanyEmployee.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
          IGetCompanyEmployeesRequest.CreateObj(new() { authorId }, includeDepartments: true));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Departments.FirstOrDefault();
        }

        _logger.LogWarning("Can not find department contain user with Id: '{authorId}'", authorId);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, errorMessage);

        errors.Add(errorMessage);
      }

      return null;
    }

    //TODO rework
    private async Task<(bool hasRights, DepartmentData department)> Authorization(Guid taskProjectId, List<string> errors)
    {
      List<DbProjectUser> projectUsers = _userRepository
        .GetProjectUsers(taskProjectId, false)
        .ToList();

      Guid requestUserId = _httpContext.GetUserId();

      DepartmentData department = await GetDepartment(requestUserId, errors);

      if (department != null)
      {
        if (department.DirectorUserId == requestUserId)
        {
          return (true, department);
        }
      }

      if (_accessValidator.IsAdmin(requestUserId))
      {
        return (true, department);
      }

      return (projectUsers.FirstOrDefault(x => x.UserId == requestUserId) != null, department);
    }

    public GetTaskCommand(
      ITaskRepository taskRepository,
      IUserRepository userRepository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      ITaskResponseMapper taskResponseMapper,
      ITaskInfoMapper taskInfoMapper,
      ILogger<GetTaskCommand> logger,
      IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployee,
      IRequestClient<IGetUsersDataRequest> userRequestClient,
      IConnectionMultiplexer cache)
    {
      _taskRepository = taskRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _httpContext = httpContextAccessor.HttpContext;
      _taskResponseMapper = taskResponseMapper;
      _taskInfoMapper = taskInfoMapper;
      _logger = logger;
      _rcGetCompanyEmployee = rcGetCompanyEmployee;
      _usersDataRequestClient = userRequestClient;
      _cache = cache;
    }

    public async Task<OperationResultResponse<TaskResponse>> Execute(Guid taskId, bool isFullModel = true)
    {
      var errors = new List<string>();

      DbTask task = _taskRepository.Get(taskId, isFullModel);

      (bool hasRights, DepartmentData department) = await Authorization(task.ProjectId, errors);

      if (!hasRights)
      {
        _httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<TaskResponse>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      List<Guid> userIds = new()
      {
        task.CreatedBy,
      };

      if (task.AssignedTo.HasValue)
      {
        userIds.Add(task.AssignedTo.Value);
      }

      if (task.ParentTask != null)
      {
        userIds.Add(task.ParentTask.CreatedBy);
      }

      Guid? parentTaskAssignedTo = task.ParentTask?.AssignedTo;
      if (parentTaskAssignedTo != null && parentTaskAssignedTo != Guid.Empty)
      {
        userIds.Add(parentTaskAssignedTo.Value);
      }

      List<UserData> usersDataResponse = new();
      try
      {
        var res = _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
          IGetUsersDataRequest.CreateObj(userIds));
        usersDataResponse = res.Result.Message.Body.UsersData;
      }
      catch (Exception exc)
      {
        errors.Add($"Can not get users info for UserIds {string.Join('\n', userIds)}. Please try again later.");

        _logger.LogWarning(exc, "Exception on get user information.");
      }

      List<TaskInfo> subtasksInfo = new();
      if (task.Subtasks != null)
      {
        foreach (var dbSubtask in task.Subtasks)
        {
          subtasksInfo.Add(
            _taskInfoMapper.Map(
              dbSubtask,
              usersDataResponse.FirstOrDefault(x => x.Id == dbSubtask.AssignedTo),
              usersDataResponse.FirstOrDefault(x => x.Id == dbSubtask.CreatedBy)));
        }
      }

      TaskResponse response = _taskResponseMapper.Map(
        task,
        usersDataResponse.FirstOrDefault(x => x.Id == task.CreatedBy),
        usersDataResponse.FirstOrDefault(x => parentTaskAssignedTo != null && x.Id == parentTaskAssignedTo),
        usersDataResponse.FirstOrDefault(x => task.ParentTask != null && x.Id == task.ParentTask.CreatedBy),
        department,
        usersDataResponse.FirstOrDefault(x => x.Id == task.AssignedTo),
        subtasksInfo);

      return new OperationResultResponse<TaskResponse>()
      {
        Status = errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess,
        Body = response,
        Errors = errors
      };
    }
  }
}
