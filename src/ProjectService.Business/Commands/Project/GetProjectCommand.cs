using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
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

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
    public class GetProjectCommand : IGetProjectCommand
    {
        private readonly ILogger<GetProjectCommand> _logger;
        private readonly IProjectRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectResponseMapper _projectResponseMapper;
        private readonly IProjectUserInfoMapper _projectUserInfoMapper;
        private readonly IProjectFileInfoMapper _projectFileInfoMapper;
        private readonly IDepartmentInfoMapper _departmentInfoMapper;
        private readonly IRequestClient<IGetDepartmentRequest> _departmentRequestClient;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
        private readonly IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> _rcGetUsersDepartmentsUsersPositions;

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

        private List<ProjectUserInfo> GetProjectUsers(IEnumerable<DbProjectUser> projectUsers, List<string> errors)
        {
            string errorMessage = null;

            List<ProjectUserInfo> projectUsersInfo = new();

            try
            {
                List<Guid> userIds = projectUsers.Select(x => x.UserId).Distinct().ToList();

                errorMessage = $"Can not get users info for UserIds {string.Join('\n', userIds)}. Please try again later.";

                IOperationResult<IGetUsersDataResponse> usersDataResponse =
                    _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
                        IGetUsersDataRequest.CreateObj(userIds)).Result.Message;

                if (usersDataResponse.IsSuccess && usersDataResponse.Body.UsersData.Any())
                {
                    var userPositionsAndDepartments = GetUserDepartmentsAndPositions(userIds, errors);

                    List<DbProjectUser> projectUsersForCount = _userRepository.Find(userIds);

                    projectUsersInfo = projectUsers
                        .Select(pu => _projectUserInfoMapper.Map(
                            usersDataResponse.Body.UsersData.FirstOrDefault(x => x.Id == pu.UserId),
                            userPositionsAndDepartments?.UsersPosition.FirstOrDefault(p => p.UserIds.Any(id => id == pu.UserId)),
                            userPositionsAndDepartments?.UsersDepartment.FirstOrDefault(d => d.UserIds.Any(id => id == pu.UserId)),
                            pu,
                            projectUsersForCount.Where(u => u.UserId == pu.UserId).Count()))
                        .ToList();

                    return projectUsersInfo;
                }
                else if (usersDataResponse.Errors != null)
                {
                    errors.Add(errorMessage);

                    _logger.LogWarning(
                        $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Errors)}.");
                }
            }
            catch (Exception exc)
            {
                errors.Add(errorMessage);

                _logger.LogError(exc, "Exception on get user information.");
            }

            return projectUsersInfo;
        }

        private IGetUsersDepartmentsUsersPositionsResponse GetUserDepartmentsAndPositions(
            List<Guid> userIds,
            List<string> errors)
        {
            string errorMessage = "Can not get user's departments and positions. Please try again later.";
            const string logMessage = "Can not get user's departments and positions for users {UserIds}. Please try again later.";

            try
            {
                var request = IGetUsersDepartmentsUsersPositionsRequest.CreateObj(userIds, includeDepartments: true, includePositions: true);
                var response = _rcGetUsersDepartmentsUsersPositions
                    .GetResponse<IOperationResult<IGetUsersDepartmentsUsersPositionsResponse>>(request)
                    .Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }
                else
                {
                    _logger.LogWarning("Errors while getting users departments and positions for users {UserIds}. Reason: {Errors}",
                        string.Join(", ", userIds),
                        string.Join('\n', response.Message.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, userIds);
            }

            errors.Add(errorMessage);

            return null;
        }

        public GetProjectCommand(
            ILogger<GetProjectCommand> logger,
            IProjectRepository repository,
            IUserRepository userRepository,
            IProjectResponseMapper projectResponsMapper,
            IProjectUserInfoMapper projectUserInfoMapper,
            IProjectFileInfoMapper projectFileInfoMapper,
            IDepartmentInfoMapper departmentInfoMapper,
            IRequestClient<IGetDepartmentRequest> departmentRequestClient,
            IRequestClient<IGetUsersDataRequest> usersDataRequestClient,
            IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> rcGetUsersDepartmentsUsersPositions)
        {
            _logger = logger;
            _repository = repository;
            _userRepository = userRepository;
            _projectResponseMapper = projectResponsMapper;
            _projectUserInfoMapper = projectUserInfoMapper;
            _projectFileInfoMapper = projectFileInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _departmentRequestClient = departmentRequestClient;
            _usersDataRequestClient = usersDataRequestClient;
            _rcGetUsersDepartmentsUsersPositions = rcGetUsersDepartmentsUsersPositions;
        }

        public ProjectResponse Execute(GetProjectFilter filter)
        {
            List<string> errors = new();

            var dbProject = _repository.Get(filter);

            DepartmentInfo department = null;
            if (dbProject.DepartmentId.HasValue)
            {
                department = GetDepartment(dbProject.DepartmentId.Value, errors);
            }

            var usersInfo = GetProjectUsers(dbProject.Users, errors);

            var filesInfo = dbProject.Files.Select(_projectFileInfoMapper.Map).ToList();

            return _projectResponseMapper.Map(dbProject, usersInfo, filesInfo, department, errors);
        }
    }
}