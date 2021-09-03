using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
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
using System.Net;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
    public class EditProjectCommand : IEditProjectCommand
    {
        private readonly IEditProjectValidator _validator;
        private readonly IAccessValidator _accessValidator;
        private readonly IPatchDbProjectMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateProjectCommand> _logger;

        private IGetDepartmentResponse GetDepartment(Guid? departmentId, List<string> errors)
        {
            string errorMessage = "Cannot edit project. Please try again later.";

            if (departmentId == null)
            {
                return null;
            }

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
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IRequestClient<IGetDepartmentRequest> requestClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateProjectCommand> logger)
        {
            _validator = validator;
            _accessValidator = accessValidator;
            _mapper = mapper;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _requestClient = requestClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public OperationResultResponse<bool> Execute(Guid projectId, JsonPatchDocument<EditProjectRequest> request)
        {
            _validator.ValidateAndThrowCustom(request);

            DbProject dbProject = _projectRepository.Get(new GetProjectFilter { ProjectId = projectId });

            OperationResultResponse<bool> response = new();
            Guid userId = _httpContextAccessor.HttpContext.GetUserId();

            if (!_accessValidator.IsAdmin() &&
                !_userRepository.AreUserProjectExist(userId, projectId, true) &&
                GetDepartment(dbProject.DepartmentId, response.Errors)?.DirectorUserId != userId)
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("Not enough rights.");

                return response;
            }

            foreach (Operation item in request.Operations)
            {
                if (item.path == $"/{nameof(EditProjectRequest.Name)}" &&
                    _projectRepository.IsProjectNameExist(item.value.ToString()))
                {
                    response.Status = OperationResultStatusType.Failed;
                    response.Errors.Add($"Project with name '{item.value}' already exist");
                    return response;
                }

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

            response.Body = _projectRepository.Edit(dbProject, _mapper.Map(request));
            response.Status = OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}
