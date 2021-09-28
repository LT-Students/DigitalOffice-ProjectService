using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
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
    private readonly ILogger<FindTasksCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetUsersDataRequest> _requestClient;
    private readonly IConnectionMultiplexer _cache;

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
        var response = await _requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
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
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IGetUsersDataRequest> requestClient,
      IConnectionMultiplexer cache)
    {
      _mapper = mapper;
      _logger = logger;
      _requestClient = requestClient;
      _taskRepository = taskRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _cache = cache;
    }

    public async Task<FindResultResponse<TaskInfo>> Execute(FindTasksFilter filter)
    {
      if (filter == null)
      {
        throw new ArgumentNullException(nameof(filter));
      }

      List<string> errors = new();

      var userId = _httpContextAccessor.HttpContext.GetUserId();
      var projectUsers = _userRepository.Find(userId).ToList();

      if (!(projectUsers.Any() || _accessValidator.IsAdmin()))
      {
        return new FindResultResponse<TaskInfo>();
      }

      IEnumerable<Guid> projectIds = projectUsers.Select(x => x.ProjectId);
      List<DbTask> dbTasks = _taskRepository.Find(filter, projectIds, out int totalCount).ToList();

      List<Guid> users = dbTasks.Where(x => x.AssignedTo.HasValue).Select(x => x.AssignedTo.Value).ToList();
      users.AddRange(dbTasks.Select(x => x.CreatedBy).ToList());

      List<UserData> usersData = await GetUsersDatas(users, errors);

      List<TaskInfo> tasks = new();
      foreach (var dbTask in dbTasks)
      {
        UserData assignedUser = usersData?.FirstOrDefault(x => x.Id == dbTask.AssignedTo);
        UserData author = usersData?.FirstOrDefault(x => x.Id == dbTask.CreatedBy);

        tasks.Add(_mapper.Map(dbTask, assignedUser, author));
      }

      return new FindResultResponse<TaskInfo>
      {
        TotalCount = totalCount,
        Body = tasks,
        Errors = errors
      };
    }
  }
}
