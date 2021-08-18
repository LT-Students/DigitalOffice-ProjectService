using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.File;
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
        private readonly IImageInfoMapper _imageMapper;
        private readonly IRequestClient<IGetDepartmentRequest> _departmentRequestClient;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
        private readonly IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> _rcGetUsersDepartmentsUsersPositions;
        private readonly IRequestClient<IGetImagesRequest> _rcImages;

        private DepartmentInfo GetDepartment(Guid departmentId, List<string> errors)
        {
            try
            {
                IOperationResult<IGetDepartmentResponse> departmentResponse = 
                    _departmentRequestClient.GetResponse<IOperationResult<IGetDepartmentResponse>>
                    (
                        IGetDepartmentRequest.CreateObj(null, departmentId)
                    )
                    .Result.Message;

                if (departmentResponse.IsSuccess)
                {
                    return _departmentInfoMapper.Map(departmentResponse.Body);
                }

                _logger.LogWarning(
                    $"Can not get department. Reason:{Environment.NewLine}{string.Join('\n', departmentResponse.Errors)}.");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception on get department request.");
            }

            errors.Add($"Can not get department info for DepartmentId '{departmentId}'. Please try again later.");

            return null;
        }

        private List<ImageInfo> GetImages(List<Guid> imageIds, List<string> errors)
        {
            if (imageIds == null || imageIds.Count == 0)
            {
                return new();
            }

            string errorMessage = "Can not get images. Please try again later.";
            const string logMessage = "Errors while getting images with ids: {Ids}.";

            try
            {
                IOperationResult<IGetImagesResponse> response = _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
                    IGetImagesRequest.CreateObj(imageIds)).Result.Message;

                if (response.IsSuccess)
                {
                    return response.Body.Images.Select(_imageMapper.Map).ToList();
                }
                else
                {
                    const string warningMessage = logMessage + "Errors: {Errors}";
                    _logger.LogWarning(
                        warningMessage,
                        string.Join(", ", imageIds),
                        string.Join('\n', response.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
            }

            errors.Add(errorMessage);

            return new();
        }

        private List<ProjectUserInfo> GetProjectUsersInfo(IEnumerable<DbProjectUser> projectUsers, List<string> errors)
        {
            if (!projectUsers.Any())
            {
                return new();
            }

            List<Guid> usersIds = projectUsers.Select(x => x.UserId).ToList();

            return GetProjectUsers(usersIds, projectUsers, ref errors);
        }

        private List<ProjectUserInfo> GetProjectUsers(List<Guid> usersIds, IEnumerable<DbProjectUser> projectUsers, ref List<string> errors)
        {
            try
            {
                IOperationResult<IGetUsersDataResponse> usersDataResponse =
                    _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>
                    (
                        IGetUsersDataRequest.CreateObj(usersIds)
                    )
                    .Result.Message;

                if (usersDataResponse.IsSuccess && usersDataResponse.Body.UsersData.Any())
                {
                    IGetUsersDepartmentsUsersPositionsResponse userPositionsAndDepartments =
                        GetUserDepartmentsAndPositions(usersIds, errors);

                    var images = GetImages(
                        usersDataResponse
                            .Body
                            .UsersData
                            .Where(ud => ud.ImageId.HasValue)
                            .Select(ud => ud.ImageId.Value)
                        .ToList(), errors);

                    List<DbProjectUser> projectUsersForCount = _userRepository.Find(usersIds);

                    return projectUsers
                        .Select(pu =>
                        {
                            UserData mappedUser = usersDataResponse.Body.UsersData.FirstOrDefault(x => x.Id == pu.UserId);
                            return _projectUserInfoMapper.Map(
                            mappedUser,
                            images.FirstOrDefault(i => i.Id == mappedUser.ImageId),
                            userPositionsAndDepartments?.UsersPosition.FirstOrDefault(p => p.UserIds.Any(id => id == pu.UserId)),
                            userPositionsAndDepartments?.UsersDepartment.FirstOrDefault(d => d.UserIds.Any(id => id == pu.UserId)),
                            pu,
                            projectUsersForCount.Where(u => u.UserId == pu.UserId).Count());
                        })
                        .ToList();
                }
                else if (usersDataResponse.Errors.Any())
                {
                    _logger.LogWarning(
                        $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Errors)}.");
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception on get user information.");
            }

            errors.Add($"Can not get users info for UserIds {string.Join('\n', usersIds)}. Please try again later.");

            return new();
        }

        private IGetUsersDepartmentsUsersPositionsResponse GetUserDepartmentsAndPositions(
            List<Guid> userIds,
            List<string> errors)
        {
            try
            {
                IOperationResult<IGetUsersDepartmentsUsersPositionsResponse> response = 
                    _rcGetUsersDepartmentsUsersPositions.GetResponse<IOperationResult<IGetUsersDepartmentsUsersPositionsResponse>>
                    (
                        IGetUsersDepartmentsUsersPositionsRequest.CreateObj(userIds, includeDepartments: true, includePositions: true)
                    )
                    .Result.Message;

                if (response.IsSuccess)
                {
                    return response.Body;
                }
                else
                {
                    _logger.LogWarning("Errors while getting users departments and positions for users {UserIds}. Reason: {Errors}",
                        string.Join(", ", userIds),
                        string.Join('\n', response.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, 
                    "Can not get user's departments and positions for users {UserIds}. Please try again later.", 
                    userIds);
            }

            errors.Add("Can not get user's departments and positions. Please try again later.");

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
            IImageInfoMapper imageMapper,
            IRequestClient<IGetDepartmentRequest> departmentRequestClient,
            IRequestClient<IGetUsersDataRequest> usersDataRequestClient,
            IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> rcGetUsersDepartmentsUsersPositions,
            IRequestClient<IGetImagesRequest> rcImages)
        {
            _logger = logger;
            _repository = repository;
            _userRepository = userRepository;
            _projectResponseMapper = projectResponsMapper;
            _projectUserInfoMapper = projectUserInfoMapper;
            _projectFileInfoMapper = projectFileInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _imageMapper = imageMapper;
            _departmentRequestClient = departmentRequestClient;
            _usersDataRequestClient = usersDataRequestClient;
            _rcGetUsersDepartmentsUsersPositions = rcGetUsersDepartmentsUsersPositions;
            _rcImages = rcImages;
        }

        public OperationResultResponse<ProjectResponse> Execute(GetProjectFilter filter)
        {
            OperationResultResponse<ProjectResponse> response = new();
            DepartmentInfo department = null;
            DbProject dbProject = _repository.Get(filter);

            if (dbProject.DepartmentId.HasValue)
            {
                department = GetDepartment(dbProject.DepartmentId.Value, response.Errors);
            }

            List<ProjectUserInfo> usersInfo = GetProjectUsersInfo(dbProject.Users, response.Errors);
            List<ProjectFileInfo> filesInfo = dbProject.Files.Select(_projectFileInfoMapper.Map).ToList();

            response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;
            response.Body = _projectResponseMapper.Map(dbProject, usersInfo, filesInfo, department);

            return response;
        }
    }
}