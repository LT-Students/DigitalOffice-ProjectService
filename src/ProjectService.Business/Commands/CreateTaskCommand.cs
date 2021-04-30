using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateTaskCommand : ICreateTaskCommand
    {
        private readonly ITaskRepository _repository;
        private readonly ICreateTaskValidator _validator;
        private readonly IDbTaskMapper _mapperTask;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTaskCommand(
            ITaskRepository repository,
            ICreateTaskValidator validator,
            IDbTaskMapper mapperTask,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<IGetUserDataRequest> requestClient)
        {
            _repository = repository;
            _validator = validator;
            _mapperTask = mapperTask;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<Guid> Execute(CreateTaskRequest request)
        {
            _validator.ValidateAndThrowCustom(request);

            var authorId = _httpContextAccessor.HttpContext.GetUserId();
            var dbTask = _mapperTask.Map(request, authorId);

            Guid taskId = _repository.CreateTask(dbTask);

            return new OperationResultResponse<Guid>
            {
                Body = taskId,
                Status = OperationResultStatusType.FullSuccess
            };
        }
    }

}
