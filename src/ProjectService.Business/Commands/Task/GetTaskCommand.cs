using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
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
        private readonly IRequestClient<IGetDepartmentRequest> _departmentRequestClient;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;

        private IGetDepartmentResponse GetDepartment(Guid userId, List<string> errors)
        {
            string errorMessage = "Cannot get user's department. Please try again later.";

            try
            {
                var response = _departmentRequestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(userId, null)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                errors.AddRange(response.Message.Errors);
                _logger.LogWarning(
                    "Can not find department for user with this id '{userId}': {NewLine}{errors}",
                    userId, Environment.NewLine, string.Join('\n', response.Message.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return null;
        }

        private void Authorization(Guid taskProjectId, List<string> errors, out IGetDepartmentResponse department)
        {
            List<DbProjectUser> projectUsers = _userRepository
                .GetProjectUsers(taskProjectId, false)
                .ToList();

            Guid requestUserId = _httpContext.GetUserId();

            department = GetDepartment(requestUserId, errors);

            if (_accessValidator.IsAdmin(requestUserId))
            {
                return;
            }

            if (projectUsers.FirstOrDefault(x => x.UserId == requestUserId) != null)
            {
                return;
            }

            if (department != null)
            {
                if (department.DirectorUserId == requestUserId)
                {
                    return;
                }
            }

            throw new ForbiddenException("Not enough rights.");
        }

        public GetTaskCommand(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            ITaskResponseMapper taskResponseMapper,
            ITaskInfoMapper taskInfoMapper,
            ILogger<GetTaskCommand> logger,
            IRequestClient<IGetDepartmentRequest> departmentRequestClient,
            IRequestClient<IGetUsersDataRequest> userRequestClient)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _accessValidator = accessValidator;
            _httpContext = httpContextAccessor.HttpContext;
            _taskResponseMapper = taskResponseMapper;
            _taskInfoMapper = taskInfoMapper;
            _logger = logger;
            _departmentRequestClient = departmentRequestClient;
            _usersDataRequestClient = userRequestClient;
        }

        public OperationResultResponse<TaskResponse> Execute(Guid taskId, bool isFullModel = true)
        {
            var errors = new List<string>();

            DbTask task = _taskRepository.Get(taskId, isFullModel);

            Authorization(task.ProjectId, errors, out IGetDepartmentResponse department);

            List<Guid> userIds = new()
            {
                task.AuthorId,
            };

            if (task.AssignedTo.HasValue)
            {
                userIds.Add(task.AssignedTo.Value);
            }

            if (task.ParentTask != null)
            {
                userIds.Add(task.ParentTask.AuthorId);
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
                            usersDataResponse.FirstOrDefault(x => x.Id == dbSubtask.AuthorId)));
                }
            }

            TaskResponse response = _taskResponseMapper.Map(
                task,
                usersDataResponse.FirstOrDefault(x => x.Id == task.AuthorId),
                usersDataResponse.FirstOrDefault(x => parentTaskAssignedTo != null && x.Id == parentTaskAssignedTo),
                usersDataResponse.FirstOrDefault(x => task.ParentTask != null && x.Id == task.ParentTask.AuthorId),
                department?.Name,
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