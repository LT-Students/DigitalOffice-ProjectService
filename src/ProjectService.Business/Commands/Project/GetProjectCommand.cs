using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Image;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
        private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartment;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
        private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployees;
        private readonly IRequestClient<IGetImagesRequest> _rcImages;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private DepartmentInfo GetDepartment(Guid departmentId, List<string> errors)
        {
            try
            {
                IOperationResult<IGetDepartmentsResponse> departmentResponse =
                    _rcGetDepartment.GetResponse<IOperationResult<IGetDepartmentsResponse>>(
                        IGetDepartmentsRequest.CreateObj(new() { departmentId }))
                    .Result.Message;

                if (departmentResponse.IsSuccess)
                {
                    return _departmentInfoMapper.Map(departmentResponse.Body.Departments.FirstOrDefault());
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

        private List<ImageInfo> GetUserAvatars(List<Guid> imageIds, List<string> errors)
        {
            if (imageIds == null || imageIds.Count == 0)
            {
                return null;
            }

            string logMessage = "Errors while getting images with ids: {Ids}. Errors: {Errors}";

            try
            {
                IOperationResult<IGetImagesResponse> response = _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
                    IGetImagesRequest.CreateObj(imageIds, ImageSource.User)).Result.Message;

                if (response.IsSuccess && response.Body.ImagesData != null)
                {
                    return response.Body.ImagesData.Select(_imageMapper.Map).ToList();
                }
                else
                {
                    _logger.LogWarning(
                        logMessage,
                        string.Join(", ", imageIds),
                        string.Join('\n', response.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
            }

            errors.Add("Can not get images. Please try again later.");

            return null;
        }

        private List<ImageInfo> GetProjectImages(List<Guid> imageIds, List<string> errors)
        {
            if (imageIds == null || imageIds.Count == 0)
            {
                return null;
            }

            string logMessage = "Errors while getting images with ids: {Ids}. Errors: {Errors}";

            try
            {
                IOperationResult<IGetImagesResponse> response = _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
                   IGetImagesRequest.CreateObj(imageIds, ImageSource.Project)).Result.Message;

                if (response.IsSuccess && response.Body != null)
                {
                    return response.Body.ImagesData.Select(_imageMapper.Map).ToList();
                }
                else
                {
                    _logger.LogWarning(
                        logMessage,
                        string.Join(", ", imageIds),
                        string.Join('\n', response.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
            }

            errors.Add("Can not get images. Please try again later.");

            return null;
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
                    IGetCompanyEmployeesResponse userPositionsAndDepartments =
                        GetCompanyEmployees(usersIds, errors);

                    var images = GetUserAvatars(
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
                            userPositionsAndDepartments?.Positions.FirstOrDefault(p => p.UsersIds.Any(id => id == pu.UserId)),
                            userPositionsAndDepartments?.Departments.FirstOrDefault(d => d.UsersIds.Any(id => id == pu.UserId)),
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

            return null;
        }

        private IGetCompanyEmployeesResponse GetCompanyEmployees(
            List<Guid> userIds,
            List<string> errors)
        {
            try
            {
                IOperationResult<IGetCompanyEmployeesResponse> response =
                    _rcGetCompanyEmployees.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
                        IGetCompanyEmployeesRequest.CreateObj(userIds, includeDepartments: true, includePositions: true))
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
            IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
            IRequestClient<IGetUsersDataRequest> usersDataRequestClient,
            IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployees,
            IRequestClient<IGetImagesRequest> rcImages,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _repository = repository;
            _userRepository = userRepository;
            _projectResponseMapper = projectResponsMapper;
            _projectUserInfoMapper = projectUserInfoMapper;
            _projectFileInfoMapper = projectFileInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _imageMapper = imageMapper;
            _rcGetDepartment = rcGetDepartments;
            _usersDataRequestClient = usersDataRequestClient;
            _rcGetCompanyEmployees = rcGetCompanyEmployees;
            _rcImages = rcImages;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<ProjectResponse> Execute(GetProjectFilter filter)
        {
            OperationResultResponse<ProjectResponse> response = new();
            DepartmentInfo department = null;
            DbProject dbProject = _repository.Get(filter);

            if (dbProject == null)
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add($"Project with Id {filter.ProjectId} was not found.");

                return response;
            }

            if (dbProject.DepartmentId.HasValue)
            {
                department = GetDepartment(dbProject.DepartmentId.Value, response.Errors);
            }

            List<ProjectUserInfo> usersInfo = GetProjectUsersInfo(dbProject.Users, response.Errors);
            List<ProjectFileInfo> filesInfo = dbProject.Files.Select(_projectFileInfoMapper.Map).ToList();
            List<ImageInfo> imagesinfo = GetProjectImages(dbProject.Images.Select(x => x.ImageId).ToList(), response.Errors);

            response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;
            response.Body = _projectResponseMapper.Map(dbProject, usersInfo, filesInfo, imagesinfo, department);

            return response;
        }
    }
}
