﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
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
    private readonly IRequestClient<IGetImagesRequest> _rcImages;
    private readonly IImageInfoMapper _imageMapper;

    private DepartmentData GetDepartment(Guid authorId)
    {
      try
      {
        Response<IOperationResult<IGetCompanyEmployeesResponse>> response = _rcGetCompanyEmployee.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
          IGetCompanyEmployeesRequest.CreateObj(new() { authorId }, includeDepartments: true)).Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Departments.FirstOrDefault();
        }

        _logger.LogWarning("Can not find department contain user with Id: '{authorId}'", authorId);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Cannot get department. Please try again later.");
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

    private bool Authorization(Guid taskProjectId, out DepartmentData department)
    {
      List<DbProjectUser> projectUsers = _userRepository
        .GetProjectUsers(taskProjectId, false)
        .ToList();

      Guid requestUserId = _httpContext.GetUserId();

      department = GetDepartment(requestUserId);

      if (_accessValidator.IsAdmin()
        || projectUsers.FirstOrDefault(x => x.UserId == requestUserId) != null
        || department != null && department.DirectorUserId == requestUserId)
      {
        return true;
      }

      return false;
    }

    private List<ImageInfo> GetTaskImages(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || imageIds.Any())
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

    private List<Guid> GetUsersIds(DbTask task, Guid? parentTaskAssignedTo)
    {
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

      if (parentTaskAssignedTo != null && parentTaskAssignedTo != Guid.Empty)
      {
        userIds.Add(parentTaskAssignedTo.Value);
      }

      return userIds;
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
      IImageInfoMapper imageMapper)
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
    }

    public OperationResultResponse<TaskResponse> Execute(Guid taskId, bool isFullModel = true)
    {
      OperationResultResponse<TaskResponse> response = new OperationResultResponse<TaskResponse>();
      DbTask task = _taskRepository.Get(taskId, isFullModel);

      if (task == null)
      {
        _httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add($"Task with Id {taskId} was not found.");

        return response;
      }

      if (!Authorization(task.ProjectId, out DepartmentData department))
      {
        _httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      Guid? parentTaskAssignedTo = task.ParentTask?.AssignedTo;
      List<Guid> userIds = GetUsersIds(task, parentTaskAssignedTo);

      List<UserData> usersDataResponse = GetUsersInfo(userIds, response.Errors);

      List<ImageInfo> imagesInfo = GetTaskImages(task.Images.Select(x => x.ImageId).ToList(), response.Errors);

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

      response.Body = _taskResponseMapper.Map(
        task,
        usersDataResponse.FirstOrDefault(x => x.Id == task.CreatedBy),
        usersDataResponse.FirstOrDefault(x => parentTaskAssignedTo != null && x.Id == parentTaskAssignedTo),
        usersDataResponse.FirstOrDefault(x => task.ParentTask != null && x.Id == task.ParentTask.CreatedBy),
        department,
        usersDataResponse.FirstOrDefault(x => x.Id == task.AssignedTo),
        subtasksInfo,
        imagesInfo);

      response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
