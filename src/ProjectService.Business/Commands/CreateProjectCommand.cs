using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
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
        private readonly IRequestClient<IGetDepartmentRequest> _requestClient;

        private IGetDepartmentResponse GetDepartment(Guid departmentId, List<string> errors)
        {
            string errorMessage = "Cannot add project. Please try again later.";

            try
            {
                var response = _requestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                IGetDepartmentRequest.CreateObj(null, departmentId)).Result;

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

        public CreateProjectCommand(
            IProjectRepository repository,
            ICreateProjectValidator validator,
            IAccessValidator accessValidator,
            IDbProjectMapper dbProjectMapper,
            IProjectInfoMapper projectInfoMapper,
            ILogger<CreateProjectCommand> logger,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<IGetDepartmentRequest> requestClient)
        {
            _logger = logger;
            _validator = validator;
            _repository = repository;
            _requestClient = requestClient;
            _dbProjectMapper = dbProjectMapper;
            _accessValidator = accessValidator;
            _projectInfoMapper = projectInfoMapper;
            _httpContextAccessor = httpContextAccessor;
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

            return new OperationResultResponse<ProjectInfo>
            {
                Body = projectInfo,
                Status = OperationResultStatusType.FullSuccess
            };
        }
    }
}
