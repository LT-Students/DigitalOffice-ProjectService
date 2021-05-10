using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetProjectByIdCommand : IGetProjectByIdCommand
    {
        private readonly ILogger<GetProjectByIdCommand> _logger;
        private readonly IProjectRepository _repository;
        private readonly IProjectExpandedResponseMapper _projectExpandedResponseMapper;
        private readonly IProjectUserInfoMapper _projectUserInfoMapper;
        private readonly IProjectFileInfoMapper _projectFileInfoMapper;
        private readonly IDepartmentInfoMapper _departmentInfoMapper;
        private readonly IRequestClient<IGetDepartmentRequest> _departmentRequestClient;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;

        private DepartmentInfo GetDepartment(Guid departmentId)
        {
            DepartmentInfo department = null;

            try
            {
                var departmentResponse = _departmentRequestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, departmentId)).Result;

                if (departmentResponse.Message.IsSuccess)
                {
                    //TODO Add mapper
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

            return department;
        }

        private List<ProjectUserInfo> GetProjectUsers(IEnumerable<DbProjectUser> projectUsers, bool showNotActiveUsers)
        {
            List<ProjectUserInfo> projectUsersInfo = new();

            try
            {
                List<Guid> userIds;

                if (showNotActiveUsers == true)
                {
                    userIds = projectUsers.Select(x => x.Id).Distinct().ToList();
                }
                else
                {
                    userIds = projectUsers.Where(x => x.IsActive == true).Select(x => x.Id).Distinct().ToList();
                }

                var usersDataResponse = _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
                    IGetUsersDataRequest.CreateObj(userIds)).Result;

                if (usersDataResponse.Message.IsSuccess)
                {
                    var usersData = usersDataResponse.Message.Body.UsersData;

                    projectUsersInfo = projectUsers
                        .Select(pu => _projectUserInfoMapper.Map(usersData.First(x => x.Id == pu.Id), pu))
                        .ToList();
                }
                else
                {
                    _logger.LogWarning(
                        $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Message.Errors)}.");
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception on get user information.");
            }

            return projectUsersInfo;
        }

        public GetProjectByIdCommand(
            ILogger<GetProjectByIdCommand> logger,
            IProjectRepository repository,
            IProjectExpandedResponseMapper projectExpandedResponsMapper,
            IProjectUserInfoMapper projectUserInfoMapper,
            IProjectFileInfoMapper projectFileInfoMapper,
            IDepartmentInfoMapper departmentInfoMapper,
            IRequestClient<IGetDepartmentRequest> departmentRequestClient,
            IRequestClient<IGetUsersDataRequest> usersDataRequestClient)
        {
            _logger = logger;
            _repository = repository;
            _projectExpandedResponseMapper = projectExpandedResponsMapper;
            _projectUserInfoMapper = projectUserInfoMapper;
            _projectFileInfoMapper = projectFileInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _departmentRequestClient = departmentRequestClient;
            _usersDataRequestClient = usersDataRequestClient;
        }

        public ProjectExpandedResponse Execute(GetProjectFilter filter)
        {
            var dbProject = _repository.GetProject(filter);

            var department = GetDepartment(dbProject.DepartmentId);

            var showNotActiveUsers = filter.ShowNotActiveUsers == true;
            var usersInfo = GetProjectUsers(dbProject.Users, showNotActiveUsers);

            var filesInfo = dbProject.Files.Select(_projectFileInfoMapper.Map).ToList();

            return _projectExpandedResponseMapper.Map(dbProject, usersInfo, filesInfo, department);
        }
    }
}