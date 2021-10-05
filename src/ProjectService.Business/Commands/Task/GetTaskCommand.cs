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
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Image;
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
    private readonly IRequestClient<IGetImagesRequest> _rcImages;
    private readonly IImageInfoMapper _imageMapper;

    private async Task<DepartmentData> GetDepartment(Guid authorId, List<string> errors)
    {
      RedisValue departmentFromCache = await _cache.GetDatabase(Cache.Departments).StringGetAsync(authorId.GetRedisCacheHashCode());

      if (departmentFromCache.HasValue)
      {
        _logger.LogInformation("Department was taken from the cache. Department ids: {authorId}", string.Join(", ", authorId));

        return JsonConvert.DeserializeObject<List<DepartmentData>>(departmentFromCache).FirstOrDefault();
      }

      return await GetDepartmentThroughBroker(authorId, errors);
    }

    private async Task<DepartmentData> GetDepartmentThroughBroker(Guid authorId, List<string> errors)
    {
      string errorMessage = "Cannot get department. Please try again later.";

      try
      {
        Response<IOperationResult<IGetCompanyEmployeesResponse>> response =
          await _rcGetCompanyEmployee.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
            IGetCompanyEmployeesRequest.CreateObj(new() { authorId }, includeDepartments: true));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Department was taken from the service. Department ids: {authorId}", string.Join(", ", authorId));

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

    private List<UserData> GetUsersInfo(List<Guid> userIds, List<string> errors)
    {
      try
      {
        IOperationResult<IGetUsersDataResponse> response = _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
          IGetUsersDataRequest.CreateObj(userIds)).Result.Message;

        if (response.IsSuccess)
        {
          return response.Body.UsersData;
        }

        errors.Add($"Can not get users info for UserIds {string.Join('\n', userIds)}. Please try again later.");
      }
      catch (Exception exc)
      {
        errors.Add($"Can not get users info for UserIds {string.Join('\n', userIds)}. Please try again later.");

        _logger.LogWarning(exc, "Exception on get user information.");
      }

      return null;
    }

    private List<ImageInfo> GetTaskImages(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
      {
        return null;
      }

      string logMessage = "Errors while getting images with ids: {Ids}. Errors: {Errors}";

      try
      {
        IOperationResult<IGetImagesResponse> response = _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imageIds, ImageSource.Project)).Result.Message;

        if (response.IsSuccess && response.Body != null)
        {
          return response.Body.ImagesData.Select(_imageMapper.Map).ToList();
        }
        else
        {
          _logger.LogWarning(
            logMessage,
            string.Join(", ", imageIds),
            string.Join('\n', response.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
      }

      errors.Add("Can not get images. Please try again later.");

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

      return (department != null && department.DirectorUserId == requestUserId
          || _accessValidator.IsAdmin(requestUserId)
          || projectUsers.FirstOrDefault(x => x.UserId == requestUserId) != null,
        department);
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
      IRequestClient<IGetImagesRequest> rcImages,
      IImageInfoMapper imageMapper,
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
      _rcImages = rcImages;
      _imageMapper = imageMapper;
      _cache = cache;
    }

    public async Task<OperationResultResponse<TaskResponse>> Execute(Guid taskId, bool isFullModel = true)
    {
      List<string> errors = new();

      DbTask task = _taskRepository.Get(taskId, isFullModel);

      (bool hasRights, DepartmentData department) = await Authorization(task.ProjectId, errors);

      if (!hasRights)
      {
        _httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        errors.Add("Not enough rights.");

        return new OperationResultResponse<TaskResponse>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      List<Guid> userIds = new() { task.CreatedBy };

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

      List<UserData> usersDataResponse = GetUsersInfo(userIds, errors);

      List<ImageInfo> imagesInfo = GetTaskImages(task.Images.Select(x => x.ImageId).ToList(), errors);

      List<TaskInfo> subtasksInfo = new();
      if (task.Subtasks != null)
      {
        foreach (DbTask dbSubtask in task.Subtasks)
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
        subtasksInfo,
        imagesInfo);

      return new OperationResultResponse<TaskResponse>()
      {
        Status = errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess,
        Body = response,
        Errors = errors
      };
    }
  }
}
