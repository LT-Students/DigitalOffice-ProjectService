using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class EditTaskCommand : IEditTaskCommand
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskPropertyRepository _taskPropertyRepository;
        private readonly IEditTaskValidator _validator;
        private readonly IAccessValidator _accessValidator;
        private readonly IPatchDbTaskMapper _mapper;
        private readonly ILogger<EditTaskCommand> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public EditTaskCommand(
            ITaskRepository taskRepository,
            ITaskPropertyRepository taskPropertyRepository,
            IEditTaskValidator validator,
            IAccessValidator accessValidator,
            IPatchDbTaskMapper mapper,
            ILogger<EditTaskCommand> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _taskRepository = taskRepository;
            _taskPropertyRepository = taskPropertyRepository;
            _validator = validator;
            _accessValidator = accessValidator;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }   
        
        public OperationResultResponse<bool> Execute(Guid taskId, JsonPatchDocument<EditTaskRequest> patch)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects))) //TODO add from kernel AddEditRemoveTasks
            {
                throw new ForbiddenException("Not enough rights.");
            }
            
            var errors = new List<string>();

            _validator.ValidateAndThrowCustom(patch);

            var dbTaskPatch = _mapper.Map(patch);
            _taskRepository.Edit(taskId, dbTaskPatch);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true,
                Errors = errors
            };
        }
    }
}