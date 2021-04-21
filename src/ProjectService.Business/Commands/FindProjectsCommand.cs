using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindProjectsCommand : IFindProjectsCommand
    {
        private readonly ILogger<FindProjectsCommand> _logger;
        private readonly IProjectRepository _repository;
        private readonly IFindProjectsResponseMapper _responseMapper;
        private readonly IFindDbProjectFilterMapper _filterMapper;
        private readonly IRequestClient<IFindDepartmentsRequest> _requestClient;

        private List<Guid> FindDepartment(string departmentName, List<string> errors)
        {
            List<Guid> departmentId = new List<Guid>();

            string errorMessage = $"Can not find departments now. Please try again later.";

            try
            {
                var findDepartmentRequest = IFindDepartmentsRequest.CreateObj(departmentName);
                var response = _requestClient.GetResponse<IOperationResult<IDepartmentsResponse>>(findDepartmentRequest).Result;
                if (response.Message.IsSuccess)
                {
                    departmentId.AddRange(response.Message.Body.DepartmentIds);
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

            return departmentId;
        }

        public FindProjectsCommand(
            ILogger<FindProjectsCommand> logger,
            IProjectRepository repository,
            IFindProjectsResponseMapper responseMapper,
            IFindDbProjectFilterMapper filterMapper,
            IRequestClient<IFindDepartmentsRequest> requestClient)
        {
            _logger = logger;
            _repository = repository;
            _responseMapper = responseMapper;
            _filterMapper = filterMapper;
            _requestClient = requestClient;
        }

        public ProjectsResponse Execute(FindProjectsFilter filter, int skipCount, int takeCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<string> errors = new();

            List<Guid> departmentIds = new List<Guid>();

            if(filter.DepartmentName != null)
            {
                departmentIds = FindDepartment(filter.DepartmentName, errors);
            }

            var dbFilter = _filterMapper.Map(filter, departmentIds);

            List<DbProject> dbProject = _repository.FindProjects(dbFilter, skipCount, takeCount, out int totalCount);

            var response = _responseMapper.Map(dbProject, totalCount, filter.DepartmentName, errors);

            return response;
        }
    }
}
