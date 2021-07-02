using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
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
    public class GetProjectCommand : IGetProjectCommand
    {
        private readonly ILogger<GetProjectCommand> _logger;
        private readonly IProjectRepository _repository;
        private readonly IProjectResponseMapper _projectResponseMapper;
        private readonly IProjectUserInfoMapper _projectUserInfoMapper;
        private readonly IProjectFileInfoMapper _projectFileInfoMapper;
        private readonly IDepartmentInfoMapper _departmentInfoMapper;
        private readonly IRequestClient<IGetDepartmentRequest> _departmentRequestClient;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;

        private DepartmentInfo GetDepartment(Guid departmentId, List<string> errors)
        {
            string errorMessage = $"Can not get department info for DepartmentId '{departmentId}'. Please try again later.";

            DepartmentInfo department = null;

            try
            {
                var departmentResponse = _departmentRequestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, departmentId)).Result;

                if (departmentResponse.Message.IsSuccess)
                {
                    department = _departmentInfoMapper.Map(departmentResponse.Message.Body);
                }
            }
            catch (Exception exc)
            {
                errors.Add(errorMessage);

                _logger.LogError(exc, "Exception on get department request.");
            }

            return department;
        }

        private List<ProjectUserInfo> GetProjectUsers(IEnumerable<DbProjectUser> projectUsers, bool showNotActiveUsers, List<string> errors)
        {
            string errorMessage = null;

            List<ProjectUserInfo> projectUsersInfo = new();

            try
            {
                List<Guid> userIds;

                if (showNotActiveUsers)
                {
                    userIds = projectUsers.Select(x => x.UserId).Distinct().ToList();
                }
                else
                {
                    userIds = projectUsers.Where(x => x.IsActive == true).Select(x => x.UserId).Distinct().ToList();
                }

                errorMessage = $"Can not get users info for UserIds {string.Join('\n', userIds)}. Please try again later.";

                var usersDataResponse = _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
                    IGetUsersDataRequest.CreateObj(userIds)).Result;

                if (usersDataResponse.Message.IsSuccess && usersDataResponse.Message.Body.UsersData.Any())
                {
                    projectUsersInfo = projectUsers
                        .Select(pu => _projectUserInfoMapper.Map(
                            usersDataResponse.Message.Body.UsersData.FirstOrDefault(x => x.Id == pu.UserId), pu))
                        .ToList();

                    return projectUsersInfo;
                }
                else if (usersDataResponse.Message.Errors != null)
                {
                    errors.Add(errorMessage);

                    _logger.LogWarning(
                        $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Message.Errors)}.");
                }
            }
            catch (Exception exc)
            {
                errors.Add(errorMessage);

                _logger.LogError(exc, "Exception on get user information.");
            }

            return projectUsersInfo;
        }

        public GetProjectCommand(
            ILogger<GetProjectCommand> logger,
            IProjectRepository repository,
            IProjectResponseMapper projectResponsMapper,
            IProjectUserInfoMapper projectUserInfoMapper,
            IProjectFileInfoMapper projectFileInfoMapper,
            IDepartmentInfoMapper departmentInfoMapper,
            IRequestClient<IGetDepartmentRequest> departmentRequestClient,
            IRequestClient<IGetUsersDataRequest> usersDataRequestClient)
        {
            _logger = logger;
            _repository = repository;
            _projectResponseMapper = projectResponsMapper;
            _projectUserInfoMapper = projectUserInfoMapper;
            _projectFileInfoMapper = projectFileInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _departmentRequestClient = departmentRequestClient;
            _usersDataRequestClient = usersDataRequestClient;
        }

        public ProjectResponse Execute(GetProjectFilter filter)
        {
            List<string> errors = new();

            var dbProject = _repository.GetProject(filter);

            var department = GetDepartment(dbProject.DepartmentId, errors);

            var showNotActiveUsers = filter.ShowNotActiveUsers == true;
            var usersInfo = GetProjectUsers(dbProject.Users, showNotActiveUsers, errors);

            var filesInfo = dbProject.Files.Select(_projectFileInfoMapper.Map).ToList();

            return _projectResponseMapper.Map(dbProject, usersInfo, filesInfo, department, errors);
        }
    }
}