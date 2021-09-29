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
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
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
using Newtonsoft.Json;
using StackExchange.Redis;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
  public class FindTasksCommand : IFindTasksCommand
  {
    private readonly ITaskInfoMapper _mapper;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IBaseFindFilterValidator _findFilterValidator;
    private readonly ILogger<FindTasksCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetUsersDataRequest> _rcGetUsers;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployee;
    private readonly IConnectionMultiplexer _cache;

    private async Task<DepartmentData> GetDepartment(Guid authorId, List<string> errors)
    {
      string errorMessage = "Cannot create task. Please try again later.";

      try
      {
        Response<IOperationResult<IGetCompanyEmployeesResponse>> response = await _rcGetCompanyEmployee.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
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

    private async Task<List<UserData>> GetUsersDatas(List<Guid> usersIds, List<string> errors)
    {
      if (usersIds == null && !usersIds.Any())
      {
        return null;
      }

      RedisValue usersFromCache = await _cache.GetDatabase(Cache.Users).StringGetAsync(usersIds.GetRedisCacheHashCode());

      if (usersFromCache.HasValue)
      {
        return JsonConvert.DeserializeObject<List<UserData>>(usersFromCache);
      }

      return await GetUsersDatasThroughBroker(usersIds, errors);
    }

    private async Task<List<UserData>> GetUsersDatasThroughBroker(List<Guid> usersIds, List<string> errors)
    {
      string errorMessage = "Can not find user data. Please try again later.";

      try
      {
        Response<IOperationResult<IGetUsersDataResponse>> response = await _rcGetUsers.GetResponse<IOperationResult<IGetUsersDataResponse>>(
          IGetUsersDataRequest.CreateObj(usersIds));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.UsersData;
        }

        errors.AddRange(response.Message.Errors);

        _logger.LogWarning("Can not find user data with this id {UserId}: " +
          $"{Environment.NewLine}{string.Join('\n', response.Message.Errors)}", usersIds);
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
      IBaseFindFilterValidator findFilterValidator,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IGetUsersDataRequest> rcGetUsers,
      IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployee,
      IConnectionMultiplexer cache)
    {
      _mapper = mapper;
      _logger = logger;
      _rcGetUsers = rcGetUsers;
      _rcGetCompanyEmployee = rcGetCompanyEmployee;
      _taskRepository = taskRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _findFilterValidator = findFilterValidator;
      _httpContextAccessor = httpContextAccessor;
      _cache = cache;
    }

    public async Task<FindResultResponse<TaskInfo>> Execute(FindTasksFilter filter)
    {
      FindResultResponse<TaskInfo> response = new();
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      List<DbProjectUser> projectUsers = _userRepository.Find(userId).ToList();

      if (!_accessValidator.IsAdmin()
        && !_accessValidator.HasRights(Rights.AddEditRemoveProjects)
        && !projectUsers.Any()
        && (await GetDepartment(userId, response.Errors))?.DirectorUserId != userId)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      if (!_findFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      IEnumerable<Guid> projectIds = projectUsers.Select(x => x.ProjectId);
      List<DbTask> dbTasks = _taskRepository.Find(filter, projectIds, out int totalCount).ToList();

      List<Guid> users = dbTasks.Where(x => x.AssignedTo.HasValue).Select(x => x.AssignedTo.Value).ToList();
      users.AddRange(dbTasks.Select(x => x.CreatedBy).ToList());

      List<UserData> usersData = await GetUsersDatas(users, response.Errors);

      List<TaskInfo> tasks = new();
      foreach (DbTask dbTask in dbTasks)
      {
        UserData assignedUser = usersData?.FirstOrDefault(x => x.Id == dbTask.AssignedTo);
        UserData author = usersData?.FirstOrDefault(x => x.Id == dbTask.CreatedBy);

        tasks.Add(_mapper.Map(dbTask, assignedUser, author));
      }

      response.TotalCount = totalCount;
      response.Body = tasks;

      return response;
    }
  }
}
