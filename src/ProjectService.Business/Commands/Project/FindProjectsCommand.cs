using LT.DigitalOffice.Kernel.Broker;
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
        private readonly IRequestClient<IFindDepartmentsRequest> _findDepartmentsRequestClient;

        private Dictionary<Guid, string> GetDepartmentsNames(List<DbProject> dbProjects, List<string> errors)
        {
            Dictionary<Guid, string> departmentNames = new Dictionary<Guid, string>();

            string errorMessage = "Can not find departments names now. Please try again later.";

            try
            {
                List<Guid> departmentIds = dbProjects.Where(p => p.DepartmentId.HasValue).Select(p => p.DepartmentId.Value).ToList();

                if (!departmentIds.Any())
                {
                    return departmentNames;
                }

                Response<IOperationResult<IFindDepartmentsResponse>> response = _findDepartmentsRequestClient
                    .GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        IFindDepartmentsRequest.CreateObj(departmentIds)).Result;
                if (response.Message.IsSuccess)
                {
                    departmentNames = response.Message.Body.IdNamePairs;
                }
                else
                {
                    _logger.LogWarning(string.Join(", ", response.Message.Errors));

                    errors.AddRange(response.Message.Errors);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return departmentNames;
        }

        public FindProjectsCommand(
            ILogger<FindProjectsCommand> logger,
            IProjectRepository repository,
            IFindProjectsResponseMapper responseMapper,
            IRequestClient<IFindDepartmentsRequest> findDepartmentsRequestClient)
        {
            _logger = logger;
            _repository = repository;
            _responseMapper = responseMapper;
            _findDepartmentsRequestClient = findDepartmentsRequestClient;
        }

        public FindResponse<ProjectInfo> Execute(FindProjectsFilter filter, int skipCount, int takeCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<string> errors = new();

            List<DbProject> dbProject = _repository.Find(filter, skipCount, takeCount, out int totalCount);

            Dictionary<Guid, string> departmentsNames = GetDepartmentsNames(dbProject, errors);

            FindResponse<ProjectInfo> response = _responseMapper.Map(dbProject, totalCount, departmentsNames, errors);

            return response;
        }
    }
}
