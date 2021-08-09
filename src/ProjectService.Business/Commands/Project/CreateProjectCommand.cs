using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
    public class CreateProjectCommand : ICreateProjectCommand
    {
        private readonly IProjectRepository _repository;
        private readonly IDbProjectMapper _dbProjectMapper;
        private readonly IAccessValidator _accessValidator;
        private readonly ICreateProjectValidator _validator;
        private readonly ILogger<CreateProjectCommand> _logger;
        private readonly IProjectInfoMapper _projectInfoMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<IGetDepartmentRequest> _rcGetDepartment;
        private readonly IRequestClient<ICreateWorkspaceRequest> _rcCreateWorkspace;
        private readonly IRequestClient<ICheckUsersExistence> _rcCheckUsersExistence;

        private IGetDepartmentResponse GetDepartment(Guid departmentId, List<string> errors)
        {
            string errorMessage = "Cannot add project. Please try again later.";

            try
            {
                var response = _rcGetDepartment.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, departmentId), timeout: TimeSpan.FromSeconds(2)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                _logger.LogWarning($"Can not find department with this id '{departmentId}': " +
                    $"{Environment.NewLine}{string.Join('\n', response.Message.Errors)}");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return null;
        }

        private ICheckUsersExistence CheckUserExistence(List<Guid> userIds, List<string> errors)
        {
            string errorMessage = "Failed to check the existing users. Please try again later ";
            string logMessage = "Cannot check existing users {userIds}";

            try
            {
                var response = _rcCheckUsersExistence.GetResponse<IOperationResult<ICheckUsersExistence>>(
                    ICheckUsersExistence.CreateObj(userIds)).Result;
                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                _logger.LogWarning($"Can not find userds with this Ids '{userIds}': " +
                    $"{Environment.NewLine}{string.Join('\n', response.Message.Errors)}");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage);

                errors.Add(errorMessage);
            }
            return null;
        }

        private void CreateWorkspace(string projectName, Guid creatorId, List<Guid> users, List<string> errors)
        {
            string errorMessage = $"Failed to create a workspace for the project {projectName}";
            string logMessage = "Cannot create workspace for project {name}";

            try
            {
                if (!users.Contains(creatorId))
                {
                    users.Add(creatorId);
                }

                var response = _rcCreateWorkspace.GetResponse<IOperationResult<bool>>(
                    ICreateWorkspaceRequest.CreateObj(projectName, creatorId, users), timeout: RequestTimeout.Default).Result;

                if (!(response.Message.IsSuccess && response.Message.Body))
                {
                    _logger.LogWarning(logMessage, projectName);
                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, projectName);

                errors.Add(errorMessage);
            }
        }

        public CreateProjectCommand(
            IProjectRepository repository,
            ICreateProjectValidator validator,
            IAccessValidator accessValidator,
            IDbProjectMapper dbProjectMapper,
            IProjectInfoMapper projectInfoMapper,
            ILogger<CreateProjectCommand> logger,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<IGetDepartmentRequest> rcGetDepartment,
            IRequestClient<ICreateWorkspaceRequest> rcCreateWorkspace,
            IRequestClient<ICheckUsersExistence> rcCheckUsersExistence)

        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
            _rcGetDepartment = rcGetDepartment;
            _dbProjectMapper = dbProjectMapper;
            _accessValidator = accessValidator;
            _projectInfoMapper = projectInfoMapper;
            _httpContextAccessor = httpContextAccessor;
            _rcCreateWorkspace = rcCreateWorkspace;
            _rcCheckUsersExistence = rcCheckUsersExistence;

        }

        public OperationResultResponse<ProjectInfo> Execute(ProjectRequest request)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            OperationResultResponse<ProjectInfo> response = new();

            if (_repository.IsProjectNameExist(request.Name))
            {
                response.Status = OperationResultStatusType.Conflict;
                response.Errors.Add($"Project with name '{request.Name}' already exist");
                return response;
            }

            _validator.ValidateAndThrowCustom(request);

            var existUsers = CheckUserExistence(request.Users.Select(u => u.UserId).ToList(), response.Errors);
            if (!response.Errors.Any() && existUsers.UserIds.Count() != request.Users.Count())
            {
                response.Status = OperationResultStatusType.PartialSuccess;
                response.Errors.Add("Not all users exist.");
                return response;
            }
            else if (response.Errors.Any())
            {
                response.Status = OperationResultStatusType.Failed;
                return response;
            }

            IGetDepartmentResponse department = null;
            if (request.DepartmentId.HasValue)
            {
                department = GetDepartment(request.DepartmentId.Value, response.Errors);

                if (!response.Errors.Any() && department == null)
                {
                    throw new BadRequestException("Project department not found.");
                }
                else if (response.Errors.Any())
                {
                    response.Status = OperationResultStatusType.Failed;
                    return response;
                }
            }

            Guid userId = _httpContextAccessor.HttpContext.GetUserId();
            DbProject dbProject = _dbProjectMapper.Map(request, userId, existUsers.UserIds);

            _repository.CreateNewProject(dbProject);

            response.Body = _projectInfoMapper.Map(dbProject, department?.Name);

            CreateWorkspace(request.Name, userId, existUsers.UserIds, response.Errors);

            response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

            return response;
        }
    }
}
