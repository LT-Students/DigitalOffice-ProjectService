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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateTaskCommand : ICreateTaskCommand
    {
        private readonly ITaskRepository _repository;
        private readonly ICreateTaskValidator _validator;
        private readonly IDbTaskMapper _mapperTask;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;
        private readonly IAccessValidator _accessValidator;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<CreateTaskCommand> _logger;

        private IGetDepartmentResponse GetDepartment(Guid userId, List<string> errors)
        {
            string errorMessage = "Cannot create task. Please try again later.";

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

        public CreateTaskCommand(
            ITaskRepository repository,
            ICreateTaskValidator validator,
            IProjectRepository projectRepository,
            IDbTaskMapper mapperTask,
            IHttpContextAccessor httpContextAccessor,
            IAccessValidator accessValidator,
            IRequestClient<IGetDepartmentRequest> requestClient,
            ILogger<CreateTaskCommand> logger)
        {
            _repository = repository;
            _validator = validator;
            _mapperTask = mapperTask;
            _httpContextAccessor = httpContextAccessor;
            _requestClient = requestClient;
            _accessValidator = accessValidator;
            _logger = logger;
            _projectRepository = projectRepository;
        }

        public OperationResultResponse<Guid> Execute(CreateTaskRequest request)
        {
            var errors = new List<string>();

            var authorId = _httpContextAccessor.HttpContext.GetUserId();

            DbProject project = _projectRepository.GetProject(request.ProjectId);

            IGetDepartmentResponse department = GetDepartment(authorId, errors);

            bool isAdmin = _accessValidator.IsAdmin();

            bool isProjectParticipant = project?.Users.FirstOrDefault(x =>
                x.UserId == authorId) != null;

            bool isDepartmentDirector = department?.Id == project?.DepartmentId;

            if (!isAdmin && !isProjectParticipant && !isDepartmentDirector)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            var dbTask = _mapperTask.Map(request, authorId);

            Guid taskId = _repository.CreateTask(dbTask);

            return new OperationResultResponse<Guid>
            {
                Body = taskId,
                Status = OperationResultStatusType.FullSuccess,
                Errors = errors
            };
        }
    }

}
