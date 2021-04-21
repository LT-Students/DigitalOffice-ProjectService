using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class ProjectExpandedResponseMapper : IProjectExpandedResponseMapper
    {
        private readonly ILogger<ProjectExpandedResponseMapper> _logger;
        private readonly IProjectMapper _projectMapper;
        private readonly IProjectUserMapper _projectUserMapper;
        private readonly IRequestClient<IGetDepartmentRequest> _departmentRequestClient;

        public ProjectExpandedResponseMapper(
            ILogger<ProjectExpandedResponseMapper> logger,
            IProjectMapper projectMapper,
            IProjectUserMapper projectUserMapper,
            IRequestClient<IGetDepartmentRequest> departmentRequestClient)
        {
            _logger = logger;
            _projectMapper = projectMapper;
            _projectUserMapper = projectUserMapper;
            _departmentRequestClient = departmentRequestClient;
        }

        public async Task<ProjectExpandedResponse> Map(
            DbProject dbProject,
            IEnumerable<DbProjectUser> users)
        {
            DepartmentInfo department = null;

            try
            {
                var departmentResponse = await _departmentRequestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(dbProject.DepartmentId));

                if (departmentResponse.Message.IsSuccess)
                {
                    department = new DepartmentInfo
                    {
                        Id = departmentResponse.Message.Body.Id,
                        Name = departmentResponse.Message.Body.Name
                    };
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception on get department request.");
            }

            return new ProjectExpandedResponse
            {
                Project = _projectMapper.Map(dbProject),
                Department = department,
                Users = users.Select(async u => await _projectUserMapper.Map(u)).Select(t => t.Result).ToList()
            };
        }
    }
}
