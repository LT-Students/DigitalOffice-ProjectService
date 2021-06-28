using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class EditProjectCommand : IEditProjectCommand
    {
        private readonly IEditProjectValidator _validator;
        private readonly IAccessValidator _accessValidator;
        private readonly IPatchDbProjectMapper _mapper;
        private readonly IProjectRepository _repository;
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateProjectCommand> _logger;

        private IGetDepartmentResponse GetDepartment(Guid departmentId, List<string> errors)
        {
            string errorMessage = "Cannot edit project. Please try again later.";

            try
            {
                var response = _requestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                IGetDepartmentRequest.CreateObj(null, departmentId)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                _logger.LogWarning(
                    "Can not find department with this id {departmentId}: " +
                    "{Environment.NewLine}{string.Join('\n', response.Message.Errors)}", departmentId);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return null;
        }

        public EditProjectCommand(
            IEditProjectValidator validator,
            IAccessValidator accessValidator,
            IPatchDbProjectMapper mapper,
            IProjectRepository repository,
            IRequestClient<IGetDepartmentRequest> requestClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateProjectCommand> logger
        )
        {
            _validator = validator;
            _accessValidator = accessValidator;
            _mapper = mapper;
            _repository = repository;
            _requestClient = requestClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public OperationResultResponse<bool> Execute(Guid projectId, JsonPatchDocument<EditProjectRequest> request)
        {
            _validator.ValidateAndThrowCustom(request);

            var filter = new GetProjectFilter { ProjectId = projectId };

            DbProject dbProject = _repository.GetProject(filter);

            var response = new OperationResultResponse<bool>();

            if (!_accessValidator.IsAdmin() &&
                (GetDepartment(dbProject.DepartmentId, response.Errors).DirectorUserId !=
                _httpContextAccessor.HttpContext.GetUserId()))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            foreach (Operation item in request.Operations)
            {
                if (item.path == $"/{nameof(EditProjectRequest.DepartmentId)}")
                {
                    var departmentData = GetDepartment(Guid.Parse(item.value.ToString()), response.Errors);

                    if (!response.Errors.Any() && departmentData == null)
                    {
                        throw new BadRequestException("Project department not found.");
                    }
                    else if (response.Errors.Any())
                    {
                        response.Status = OperationResultStatusType.Failed;
                        return response;
                    }
                };
            }

            response.Body = _repository.Edit(dbProject, _mapper.Map(request));
            response.Status = OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}
