using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
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
        private readonly IUserRepository _userRepository;
        private readonly IEditTaskValidator _validator;
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPatchDbTaskMapper _mapper;
        private readonly ILogger<EditTaskCommand> _logger;
        private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployee;

        private DepartmentData GetDepartment(Guid authorId, List<string> errors)
        {
            string errorMessage = "Cannot create task. Please try again later.";

            try
            {
                var response = _rcGetCompanyEmployee.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
                    IGetCompanyEmployeesRequest.CreateObj(new() { authorId }, includeDepartments: true)).Result;

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

        public EditTaskCommand(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IEditTaskValidator validator,
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IPatchDbTaskMapper mapper,
            ILogger<EditTaskCommand> logger,
            IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployee)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _validator = validator;
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
            _rcGetCompanyEmployee = rcGetCompanyEmployee;
        }

        public OperationResultResponse<bool> Execute(Guid taskId, JsonPatchDocument<EditTaskRequest> patch)
        {
            var errors = new List<string>();

            DbTask task = _taskRepository.Get(taskId, false);

            Guid requestUserId = _httpContextAccessor.HttpContext.GetUserId();

            if (!_accessValidator.IsAdmin()
                && !(_userRepository.AreUserProjectExist(requestUserId, task.ProjectId))
                && !(GetDepartment(requestUserId, errors)?.DirectorUserId == requestUserId))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(patch);

            _taskRepository.Edit(task, _mapper.Map(patch));

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
