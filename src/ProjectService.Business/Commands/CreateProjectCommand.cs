using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
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

namespace LT.DigitalOffice.ProjectService.Business.Commands
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
            IRequestClient<ICreateWorkspaceRequest> rcCreateWorkspace)
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
        }

        public OperationResultResponse<ProjectInfo> Execute(ProjectRequest request)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            var errors = new List<string>();

            _validator.ValidateAndThrowCustom(request);

            IGetDepartmentResponse department = GetDepartment(request.DepartmentId, errors);
            if (!errors.Any() && department == null)
            {
                throw new BadRequestException("Project department not found.");
            }
            else if (errors.Any())
            {
                return new OperationResultResponse<ProjectInfo>
                {
                    Status = OperationResultStatusType.Failed,
                    Errors = errors
                };
            }

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var dbProject = _dbProjectMapper.Map(request, userId);

            _repository.CreateNewProject(dbProject);

            var projectInfo = _projectInfoMapper.Map(dbProject, department.Name);

            CreateWorkspace(request.Name, userId, request.Users.Select(u => u.UserId).ToList(), errors);

            return new OperationResultResponse<ProjectInfo>
            {
                Body = projectInfo,
                Status = errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess,
                Errors = errors
            };
        }
    }
}
