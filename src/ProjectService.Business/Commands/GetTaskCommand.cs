using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetTaskCommand : IGetTaskCommand
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccessValidator _accessValidator;
        private readonly HttpContext _httpContext;
        private readonly ITaskResponseMapper _mapper;
        private readonly ILogger<GetTaskCommand> _logger;
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;

        private IGetDepartmentResponse GetDepartment(Guid userId, List<string> errors)
        {
            string errorMessage = "Cannot get task. Please try again later.";

            try
            {
                var response = _requestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(userId, null)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                _logger.LogWarning(
                    "Can not find department with this id '{userId}': {NewLine}{errors}",
                    userId, Environment.NewLine, string.Join('\n', response.Message.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return null;
        }
        
        public GetTaskCommand(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            ITaskResponseMapper mapper,
            ILogger<GetTaskCommand> logger,
            IRequestClient<IGetDepartmentRequest> requestClient)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _accessValidator = accessValidator;
            _httpContext = httpContextAccessor.HttpContext;
            _mapper = mapper;
            _logger = logger;
            _requestClient = requestClient;
        }

        public TaskResponse Execute(Guid taskId)
        {
            var errors = new List<string>();

            DbTask task = _taskRepository.Get(taskId);
            
            List<DbProjectUser> projectUsers = null;
            if (task != null)
            {
                 projectUsers = _userRepository.GetProjectUsers(task.ProjectId, false).ToList();
            }

            Guid requestUserId = _httpContext.GetUserId();
            IGetDepartmentResponse department = GetDepartment(requestUserId, errors);

            bool isAdmin = _accessValidator.IsAdmin();

            bool isProjectParticipant = false;
            if (projectUsers != null)
            {
                isProjectParticipant =  projectUsers.FirstOrDefault(x =>
                    x.UserId == requestUserId) != null;
            }

            bool isDepartmentDirector = false;
            if (department != null)
            {
                isDepartmentDirector = department.DirectorUserId == requestUserId;
            }

            if (!isAdmin && !isProjectParticipant && !isDepartmentDirector)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            TaskResponse response = _mapper.Map(_taskRepository.GetFullModel(taskId));
            response.Errors = errors;

            return response;
        }
    }
}
