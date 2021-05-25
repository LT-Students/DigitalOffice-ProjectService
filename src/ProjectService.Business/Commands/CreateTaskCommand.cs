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
        private readonly IUserRepository _userRepository;
        private readonly IDbTaskMapper _mapperTask;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;
        private readonly IAccessValidator _accessValidator;
        private readonly ILogger<CreateTaskCommand> _logger;

        private IGetDepartmentResponse GetDepartment(Guid authorId, List<string> errors)
        {
            string errorMessage = "Cannot create task. Please try again later.";

            try
            {
                var response = _requestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(authorId, null)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
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

        public CreateTaskCommand(
            ITaskRepository repository,
            ICreateTaskValidator validator,
            IProjectRepository projectRepository,
            IDbTaskMapper mapperTask,
            IHttpContextAccessor httpContextAccessor,
            IAccessValidator accessValidator,
            IUserRepository userRepository,
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
            _userRepository = userRepository;
        }

        public OperationResultResponse<Guid> Execute(CreateTaskRequest request)
        {
            var errors = new List<string>();

            var authorId = _httpContextAccessor.HttpContext.GetUserId();

            List<DbProjectUser> projectUsers =
                _userRepository.GetProjectUsers(request.ProjectId, false).ToList();

            IGetDepartmentResponse department = GetDepartment(authorId, errors);

            bool isAdmin = _accessValidator.IsAdmin();

            _validator.ValidateAndThrowCustom(request);

            bool isProjectParticipant = projectUsers.FirstOrDefault(x =>
                x.UserId == authorId) != null;

            if (!isAdmin && !isProjectParticipant && department == null && !errors.Any())
            {
                throw new ForbiddenException("Not enough rights.");
            }

            Guid taskId = _repository.CreateTask(_mapperTask.Map(request));

            return new OperationResultResponse<Guid>
            {
                Body = taskId,
                Status = !errors.Any() ?
                    OperationResultStatusType.FullSuccess : OperationResultStatusType.PartialSuccess,
                Errors = errors
            };
        }
    }
}
