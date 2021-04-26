using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateTaskCommand : ICreateNewTaskCommand
    {
        private readonly ITaskRepository _repository;
        private readonly ICreateTaskValidator _validator;
        private readonly IDbTaskMapper _mapperTask;
        private readonly IAccessValidator _accessValidator;

        public CreateTaskCommand(
            ITaskRepository repository,
            ICreateTaskValidator validator,
            IDbTaskMapper mapperTask,
            IAccessValidator accessValidator)
        {
            _repository = repository;
            _validator = validator;
            _mapperTask = mapperTask;

        }

        public OperationResultResponse<Guid> Execute(CreateTaskRequest request)
        {
            _repository.IsExist(request.AuthorId, request.AssignedTo.Value, request.ProjectId, request.ParentTaskId.Value);
        }
    }

}
