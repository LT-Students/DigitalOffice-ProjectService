using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
    public class FindProjectsCommand : IFindProjectsCommand
    {
        private readonly ILogger<FindProjectsCommand> _logger;
        private readonly IProjectRepository _repository;
        private readonly IFindProjectsResponseMapper _responseMapper;
        private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;

        private List<DepartmentData> GetDepartments(List<DbProject> dbProjects, List<string> errors)
        {
            if (dbProjects == null || !dbProjects.Any())
            {
                return null;
            }

            List<Guid> departmentIds = dbProjects.Where(p => p.DepartmentId.HasValue).Select(p => p.DepartmentId.Value).ToList();

            if (!departmentIds.Any())
            {
                return null;
            }

            string errorMessage = "Cannot get departments now. Please try again later.";

            try
            {
                Response<IOperationResult<IGetDepartmentsResponse>> response = _rcGetDepartments
                    .GetResponse<IOperationResult<IGetDepartmentsResponse>>(
                        IGetDepartmentsRequest.CreateObj(departmentIds)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body.Departments;
                }

                _logger.LogWarning(string.Join(", ", response.Message.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);
            }

            errors.Add(errorMessage);

            return null;
        }

        public FindProjectsCommand(
            ILogger<FindProjectsCommand> logger,
            IProjectRepository repository,
            IFindProjectsResponseMapper responseMapper,
            IRequestClient<IGetDepartmentsRequest> rcGetDepartments)
        {
            _logger = logger;
            _repository = repository;
            _responseMapper = responseMapper;
            _rcGetDepartments = rcGetDepartments;
        }

        public FindResponse<ProjectInfo> Execute(FindProjectsFilter filter, int skipCount, int takeCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<string> errors = new();

            List<DbProject> dbProject = _repository.Find(filter, skipCount, takeCount, out int totalCount);

            List<DepartmentData> departments = GetDepartments(dbProject, errors);

            FindResponse<ProjectInfo> response = _responseMapper.Map(dbProject, totalCount, departments, errors);

            return response;
        }
    }
}
