using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class EditTaskCommand : IEditTaskCommand
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEditTaskValidator _validator;
        private readonly IAccessValidator _accessValidator;
        private readonly HttpContext _httpContext;
        private readonly IPatchDbTaskMapper _mapper;
        private readonly ILogger<EditTaskCommand> _logger;
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;
        
        private IGetDepartmentResponse GetDepartment(Guid userId, List<string> errors)
        {
            string errorMessage = "Cannot edit task. Please try again later.";

            try
            {
                var response = _requestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(userId, null)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                _logger.LogWarning($"Can not find department with this id '{userId}': " +
                                   $"{Environment.NewLine}{string.Join('\n', response.Message.Errors)}");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return null;
        }

        public EditTaskCommand(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IEditTaskValidator validator,
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IPatchDbTaskMapper mapper,
            ILogger<EditTaskCommand> logger,
            IRequestClient<IGetDepartmentRequest> requestClient)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _validator = validator;
            _accessValidator = accessValidator;
            _httpContext = httpContextAccessor.HttpContext;
            _mapper = mapper;
            _logger = logger;
            _requestClient = requestClient;
        }

        public OperationResultResponse<bool> Execute(Guid taskId, JsonPatchDocument<EditTaskRequest> patch)
        {
            _validator.ValidateAndThrowCustom(patch);

            var errors = new List<string>();

            DbTask task = _taskRepository.Get(taskId);
            DbProject project = _projectRepository.GetProject(task.ProjectId);

            Guid requestUserId = _httpContext.GetUserId();
            IGetDepartmentResponse department = GetDepartment(requestUserId, errors);

            bool isAdmin = _accessValidator.IsAdmin();
            
            bool isProjectParticipant = project?.Users.FirstOrDefault(x =>
                x.UserId == requestUserId) != null;

            bool isDepartmentDirector = false;
            if (department != null && project != null)
            {
                 isDepartmentDirector = department.Id == project.DepartmentId;
            }

            if (!isAdmin && !isProjectParticipant && !isDepartmentDirector)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            var dbTaskPatch = _mapper.Map(patch);

            _taskRepository.Edit(task, dbTaskPatch);

            return new OperationResultResponse<bool>
            {
                Status = errors.Any() ? 
                    OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess,
                Body = true,
                Errors = errors
            };
        }
    }
}
