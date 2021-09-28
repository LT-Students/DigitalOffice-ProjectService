using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
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
using Newtonsoft.Json;
using StackExchange.Redis;

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
    private readonly IConnectionMultiplexer _cache;

    private async Task<DepartmentData> GetDepartment(Guid departmentId, List<string> errors)
    {
      RedisValue departmentFromCache = await _cache.GetDatabase(Cache.Departments).StringGetAsync(departmentId.GetRedisCacheHashCode());

      if (departmentFromCache.HasValue)
      {
        return JsonConvert.DeserializeObject<List<DepartmentData>>(departmentFromCache).FirstOrDefault();
      }

      return await GetDepartmentThroughBroker(departmentId, errors);
    }

    private async Task<DepartmentData> GetDepartmentThroughBroker(Guid departmentId, List<string> errors)
    {
      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> departmentResponse =
            await _rcGetDepartment.GetResponse<IOperationResult<IGetDepartmentsResponse>>(
                IGetDepartmentsRequest.CreateObj(new() { departmentId }));

        if (departmentResponse.Message.IsSuccess)
        {
          return departmentResponse.Message.Body.Departments.FirstOrDefault();
        }

        _logger.LogWarning(
            $"Can not get department. Reason:{Environment.NewLine}{string.Join('\n', departmentResponse.Message.Errors)}.");
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Exception on get department request.");
      }

      errors.Add($"Can not get department info for DepartmentId '{departmentId}'. Please try again later.");

      return null;
    }

    private async Task<List<ImageInfo>> GetUserAvatars(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || imageIds.Count == 0)
      {
        return null;
      }

      string logMessage = "Errors while getting images with ids: {Ids}. Errors: {Errors}";

      try
      {
        Response<IOperationResult<IGetImagesResponse>> response = await _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
            IGetImagesRequest.CreateObj(imageIds, ImageSource.User));

        if (response.Message.IsSuccess && response.Message.Body.ImagesData != null)
        {
          return response.Message.Body.ImagesData.Select(_imageMapper.Map).ToList();
        }
        else
        {
          _logger.LogWarning(
              logMessage,
              string.Join(", ", imageIds),
              string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
      }

      errors.Add("Can not get images. Please try again later.");

      return null;
    }

    private async Task<List<ImageInfo>> GetProjectImages(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || imageIds.Count == 0)
      {
        return null;
      }

      string logMessage = "Errors while getting images with ids: {Ids}. Errors: {Errors}";

      try
      {
        Response<IOperationResult<IGetImagesResponse>> response = await _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
           IGetImagesRequest.CreateObj(imageIds, ImageSource.Project));

        if (response.Message.IsSuccess && response.Message.Body != null)
        {
          return response.Message.Body.ImagesData.Select(_imageMapper.Map).ToList();
        }
        else
        {
          _logger.LogWarning(
            logMessage,
            string.Join(", ", imageIds),
            string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
      }

      errors.Add("Can not get images. Please try again later.");

      return null;
    }

    private async Task<List<UserData>> GetUsersDatas(IEnumerable<DbProjectUser> projectUsers, List<string> errors)
    {
      if (projectUsers == null || !projectUsers.Any())
      {
        return null;
      }

      List<Guid> usersIds = projectUsers.Select(x => x.UserId).ToList();

      RedisValue usersFromCache = await _cache.GetDatabase(Cache.Users).StringGetAsync(usersIds.GetRedisCacheHashCode());

      if (usersFromCache.HasValue)
      {
        return JsonConvert.DeserializeObject<List<UserData>>(usersFromCache);
      }

      return await GetUsersDatasThroughBroker(usersIds, errors);
    }

    private async Task<List<UserData>> GetUsersDatasThroughBroker(List<Guid> usersIds, List<string> errors)
    {
      try
      {
        Response<IOperationResult<IGetUsersDataResponse>> usersDataResponse =
          await _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
            IGetUsersDataRequest.CreateObj(usersIds));

        if (usersDataResponse.Message.IsSuccess)
        {
          return usersDataResponse.Message.Body.UsersData;
        }
        
        _logger.LogWarning(
            $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Message.Errors)}.");
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Exception on get user information.");
      }

      errors.Add($"Can not get users info for UserIds {string.Join('\n', usersIds)}. Please try again later.");

      return null;
    }

    private async Task<(List<DepartmentData> departments, List<PositionData> positions)> GetCompanyEmployee(
      List<Guid> usersIds,
      List<string> errors)
    {
      (List<DepartmentData> departments, List<PositionData> positions) =
        await GetCompanyEmployeesFromCache(usersIds);

      IGetCompanyEmployeesResponse brokerResponse = await GetCompanyEmployeesThroughBroker(
        usersIds,
        departments == null,
        positions == null,
        errors);

      return (departments ?? brokerResponse?.Departments,
        positions ?? brokerResponse?.Positions);
    }

    private async Task<(List<DepartmentData> departments, List<PositionData> positions)> GetCompanyEmployeesFromCache(List<Guid> usersIds)
    {
      List<DepartmentData> departments = null;
      List<PositionData> positions = null;

      string key = usersIds.GetRedisCacheHashCode();

      Task<RedisValue> departmentsFromCacheTask = _cache.GetDatabase(Cache.Departments).StringGetAsync(key);
      Task<RedisValue> positionsFromCacheTask = _cache.GetDatabase(Cache.Positions).StringGetAsync(key);

      RedisValue departmentsFromCache = await departmentsFromCacheTask;
      if (departmentsFromCache.HasValue)
      {
        departments = JsonConvert.DeserializeObject<List<DepartmentData>>(departmentsFromCache);
      }

      RedisValue positionsFromCache = await positionsFromCacheTask;
      if (positionsFromCache.HasValue)
      {
        positions = JsonConvert.DeserializeObject<List<PositionData>>(positionsFromCache);
      }

      return (departments, positions);
    }

    private async Task<IGetCompanyEmployeesResponse> GetCompanyEmployeesThroughBroker(
      List<Guid> usersIds,
      bool includeDepartments,
      bool includePositions,
      List<string> errors)
    {
      if (!includeDepartments && !includePositions)
      {
        return null;
      }

      try
      {
        Response<IOperationResult<IGetCompanyEmployeesResponse>> response =
          await _rcGetCompanyEmployees.GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
            IGetCompanyEmployeesRequest.CreateObj(usersIds, includeDepartments: includeDepartments, includePositions: includePositions));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }
        else
        {
          _logger.LogWarning("Errors while getting users departments and positions for users {UserIds}. Reason: {Errors}",
              string.Join(", ", usersIds),
              string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc,
            "Can not get user's departments and positions for users {UserIds}. Please try again later.",
            usersIds);
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
      IConnectionMultiplexer cache)
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
      _cache = cache;
    }

    public async Task<OperationResultResponse<ProjectResponse>> Execute(GetProjectFilter filter)
    {
      OperationResultResponse<ProjectResponse> response = new();
      DepartmentData department = null;
      DbProject dbProject = _repository.Get(filter);

      if (dbProject.DepartmentId.HasValue)
      {
        department = await GetDepartment(dbProject.DepartmentId.Value, response.Errors);
      }

      List<UserData> usersDatas = await GetUsersDatas(dbProject.Users, response.Errors);
      List<ProjectUserInfo> usersInfo = null;
      List<Guid> usersIds = dbProject.Users.Select(u => u.UserId).Distinct().ToList();

      if (usersDatas != null && usersDatas.Any())
      {
        (List<DepartmentData> departments, List<PositionData> positions) = await GetCompanyEmployee(usersIds, response.Errors);
        List<ImageInfo> imagesInfos = await GetUserAvatars(usersDatas.Where(u => u.ImageId.HasValue).Select(u => u.ImageId.Value).ToList(), response.Errors);

        //rework
        List<DbProjectUser> projectUsersForCount = _userRepository.Find(usersIds);

        usersInfo = dbProject.Users
          .Select(pu =>
          {
            UserData mappedUser = usersDatas.FirstOrDefault(x => x.Id == pu.UserId);

            return _projectUserInfoMapper.Map(
              mappedUser,
              imagesInfos?.FirstOrDefault(i => i.Id == mappedUser.ImageId),
              positions?.FirstOrDefault(p => p.UsersIds.Any(id => id == pu.UserId)),
              departments?.FirstOrDefault(d => d.UsersIds.Any(id => id == pu.UserId)),
              pu,
              projectUsersForCount.Where(u => u.UserId == pu.UserId).Count());
          })
          .ToList();
      }

      List<ProjectFileInfo> filesInfo = dbProject.Files.Select(_projectFileInfoMapper.Map).ToList();
      List<ImageInfo> imagesinfo = await GetProjectImages(dbProject.Images.Select(x => x.ImageId).ToList(), response.Errors);

      response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;
      response.Body = _projectResponseMapper.Map(dbProject, usersInfo, filesInfo, imagesinfo, _departmentInfoMapper.Map(department));

      return response;
    }
  }
}
