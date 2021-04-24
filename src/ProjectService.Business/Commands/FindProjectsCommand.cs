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
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindProjectsCommand : IFindProjectsCommand
    {
        private readonly ILogger<FindProjectsCommand> _logger;
        private readonly IProjectRepository _repository;
        private readonly IFindProjectsResponseMapper _responseMapper;
        private readonly IFindDbProjectFilterMapper _filterMapper;
        private readonly IRequestClient<IFindDepartmentsRequest> _findDepartmentsRequestClient;
        private readonly IRequestClient<IGetDepartmentsNamesRequest> _getDepartmentsRequestClient;

        private IDictionary<Guid, string> FindDepartment(string departmentName, List<string> errors)
        {
            IDictionary<Guid, string> pairs = new Dictionary<Guid, string>();

            string errorMessage = $"Can not find departments now. Please try again later.";

            try
            {
                var findDepartmentRequest = IFindDepartmentsRequest.CreateObj(departmentName);
                var response = _findDepartmentsRequestClient.GetResponse<IOperationResult<IFindDepartmentsResponse>>(findDepartmentRequest).Result;
                if (response.Message.IsSuccess)
                {
                    foreach(var pair in response.Message.Body.IdNamePairs)
                    {
                        pairs.Add(pair);
                    }
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

            return pairs;
        }

        private IDictionary<Guid, string> GetDepartmentName(List<DbProject> dbProjects, List<string> errors)
        {
            IDictionary<Guid, string> pairs = new Dictionary<Guid, string>();

            string errorMessage = $"Can not find departments names now. Please try again later.";

            try
            {
                var getDepartmentsRequest = IGetDepartmentsNamesRequest.CreateObj(
                    dbProjects.Select(p => p.DepartmentId).ToList());
                var response = _getDepartmentsRequestClient.GetResponse<IOperationResult<IGetDepartmentsNamesResponse>>(
                    getDepartmentsRequest).Result;
                if (response.Message.IsSuccess)
                {
                    pairs = response.Message.Body.IdNamePairs;
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

            return pairs;
        }

        public FindProjectsCommand(
            ILogger<FindProjectsCommand> logger,
            IProjectRepository repository,
            IFindProjectsResponseMapper responseMapper,
            IFindDbProjectFilterMapper filterMapper,
            IRequestClient<IFindDepartmentsRequest> findDepartmentsRequestClient,
            IRequestClient<IGetDepartmentsNamesRequest> getDepartmentsRequestClient)
        {
            _logger = logger;
            _repository = repository;
            _responseMapper = responseMapper;
            _filterMapper = filterMapper;
            _findDepartmentsRequestClient = findDepartmentsRequestClient;
            _getDepartmentsRequestClient = getDepartmentsRequestClient;
        }

        public ProjectsResponse Execute(FindProjectsFilter filter, int skipCount, int takeCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<string> errors = new();

            IDictionary<Guid, string> pairs = null;

            if(filter.DepartmentName != null)
            {
                pairs = FindDepartment(filter.DepartmentName, errors);
            }

            var dbFilter = _filterMapper.Map(filter, pairs);

            List<DbProject> dbProject = _repository.FindProjects(dbFilter, skipCount, takeCount, out int totalCount);

            var departmentsNames = pairs ?? GetDepartmentName(dbProject, errors);

            var response = _responseMapper.Map(dbProject, totalCount, departmentsNames, errors);

            return response;
        }
    }
}
