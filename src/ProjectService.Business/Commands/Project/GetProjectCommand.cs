using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class GetProjectCommand : IGetProjectCommand
  {
    private readonly ILogger<GetProjectCommand> _logger;
    private readonly IProjectRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectResponseMapper _projectResponseMapper;
    private readonly IUserInfoMapper _projectUserInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IImageInfoMapper _imageMapper;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartment;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPosition;
    private readonly IRequestClient<IGetCompaniesRequest> _rcGetCompanies;
    private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
    private readonly IRequestClient<IGetImagesRequest> _rcImages;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IResponseCreator _responseCreator;

    #region private methods
    private async Task<List<DepartmentData>> GetDepartmentAsync(Guid projectId, List<Guid> usersIds, List<string> errors)
    {
      string key;

      if (usersIds != null && usersIds.Any())
      {
        key = usersIds.GetRedisCacheHashCode(projectId);
      }
      else
      {
        key = projectId.GetRedisCacheHashCode();
      }

      List<DepartmentData> departmentsFromCache = await _globalCache.GetAsync<List<DepartmentData>>(Cache.Departments, key);

      if (departmentsFromCache != null)
      {
        _logger.LogInformation($"Department was taken from the cache. Department id: {projectId}");

        return departmentsFromCache;
      }

      return await GetDepartmentThroughBrokerAsync(projectId, usersIds, errors);
    }

    private async Task<List<DepartmentData>> GetDepartmentThroughBrokerAsync(Guid projectId, List<Guid> usersIds, List<string> errors)
    {
      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> departmentResponse =
          await _rcGetDepartment.GetResponse<IOperationResult<IGetDepartmentsResponse>>(
            IGetDepartmentsRequest.CreateObj(projectsIds: new() { projectId }, usersIds: usersIds));

        if (departmentResponse.Message.IsSuccess)
        {
          _logger.LogInformation($"Departments was taken from the service.");

          return departmentResponse.Message.Body.Departments;
        }

        _logger.LogWarning(
          $"Can not get department. Reason:{Environment.NewLine}{string.Join('\n', departmentResponse.Message.Errors)}.");
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Exception on get department request.");
      }

      errors.Add($"Can not get departments info. Please try again later.");

      return null;
    }

    private async Task<List<ImageInfo>> GetUserAvatarsAsync(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
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

    private async Task<List<ImageInfo>> GetProjectImagesAsync(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
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

        _logger.LogWarning(
          logMessage,
          string.Join(", ", imageIds),
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
      }

      errors.Add("Can not get images. Please try again later.");

      return null;
    }

    private async Task<List<UserData>> GetUsersDatasAsync(IEnumerable<DbProjectUser> projectUsers, List<string> errors)
    {
      if (projectUsers == null || !projectUsers.Any())
      {
        return null;
      }

      List<Guid> usersIds = projectUsers.Select(x => x.UserId).ToList();

      List<UserData> usersFromCache = await _globalCache.GetAsync<List<UserData>>(Cache.Users, usersIds.GetRedisCacheHashCode());

      if (usersFromCache != null)
      {
        _logger.LogInformation("UsersDatas were taken from the cache. Users ids: {usersIds}", string.Join(", ", usersIds));

        return usersFromCache;
      }

      return await GetUsersDatasThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<UserData>> GetUsersDatasThroughBrokerAsync(List<Guid> usersIds, List<string> errors)
    {
      try
      {
        Response<IOperationResult<IGetUsersDataResponse>> usersDataResponse =
          await _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
            IGetUsersDataRequest.CreateObj(usersIds));

        if (usersDataResponse.Message.IsSuccess)
        {
          _logger.LogInformation("UsersDatas were taken from the service. Users ids: {usersIds}", string.Join(", ", usersIds));

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

    private async Task<List<PositionData>> GetPositionsAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      List<PositionData> positions = await _globalCache.GetAsync<List<PositionData>>(Cache.Positions, usersIds.GetRedisCacheHashCode());

      if (positions != null)
      {
        _logger.LogInformation($"Positions were taken from the cache.");

        return positions;
      }

      return await GetPositionsThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<PositionData>> GetPositionsThroughBrokerAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      try
      {
        Response<IOperationResult<IGetPositionsResponse>> response =
          await _rcGetPosition.GetResponse<IOperationResult<IGetPositionsResponse>>(
            IGetPositionsRequest.CreateObj(usersIds: usersIds));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Positions;
        }
        else
        {
          _logger.LogWarning("Errors while getting users positions. Reason: {Errors}",
            string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc,
          "Can not get user's positions. Please try again later.",
          usersIds);
      }

      errors.Add("Can not get user's positions. Please try again later.");

      return null;
    }

    private async Task<List<CompanyData>> GetCompaniesAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      List<CompanyData> companies = await _globalCache.GetAsync<List<CompanyData>>(Cache.Companies, usersIds.GetRedisCacheHashCode());

      if (companies != null)
      {
        _logger.LogInformation($"Companies were taken from the cache.");

        return companies;
      }

      return await GetCompaniesThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<CompanyData>> GetCompaniesThroughBrokerAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      try
      {
        Response<IOperationResult<IGetCompaniesResponse>> response =
          await _rcGetCompanies.GetResponse<IOperationResult<IGetCompaniesResponse>>(
            IGetCompaniesRequest.CreateObj(usersIds: usersIds));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Companies;
        }
        else
        {
          _logger.LogWarning("Errors while getting users companies. Reason: {Errors}",
            string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc,
          "Can not get user's companies. Please try again later.",
          usersIds);
      }

      errors.Add("Can not get user's companies. Please try again later.");

      return null;
    }

    #endregion

    public GetProjectCommand(
      ILogger<GetProjectCommand> logger,
      IProjectRepository repository,
      IUserRepository userRepository,
      IProjectResponseMapper projectResponsMapper,
      IUserInfoMapper projectUserInfoMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      IImageInfoMapper imageMapper,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetUsersDataRequest> usersDataRequestClient,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      IRequestClient<IGetCompaniesRequest> rcGetCompanies,
      IRequestClient<IGetImagesRequest> rcImages,
      IGlobalCacheRepository globalCache,
      IResponseCreator responseCreator)
    {
      _logger = logger;
      _repository = repository;
      _userRepository = userRepository;
      _projectResponseMapper = projectResponsMapper;
      _projectUserInfoMapper = projectUserInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _imageMapper = imageMapper;
      _rcGetDepartment = rcGetDepartments;
      _usersDataRequestClient = usersDataRequestClient;
      _rcGetPosition = rcGetPositions;
      _rcGetCompanies = rcGetCompanies;
      _rcImages = rcImages;
      _globalCache = globalCache;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<ProjectResponse>> ExecuteAsync(GetProjectFilter filter)
    {
      DbProject dbProject = await _repository.GetAsync(filter);

      if (dbProject is null)
      {
        return _responseCreator.CreateFailureResponse<ProjectResponse>(System.Net.HttpStatusCode.NotFound);
      }

      OperationResultResponse<ProjectResponse> response = new();
      DepartmentData department = null;
      List<UserInfo> usersInfo = null;

      List<UserData> usersDatas = await GetUsersDatasAsync(dbProject.Users, response.Errors);
      List<Guid> usersIds = dbProject.Users.Select(u => u.UserId).Distinct().ToList();

      if (usersDatas != null && usersDatas.Any())
      {
        var positionsTask = GetPositionsAsync(usersIds, response.Errors);
        var companiesTask = GetCompaniesAsync(usersIds, response.Errors);
        var departmentsTask = GetDepartmentAsync(dbProject.Id, usersIds, response.Errors);
        var imagesTask = GetUserAvatarsAsync(usersDatas.Where(u => u.ImageId.HasValue).Select(u => u.ImageId.Value).ToList(), response.Errors);

        await Task.WhenAll(positionsTask, departmentsTask, companiesTask, imagesTask);

        List<DepartmentData> departments = await departmentsTask;
        List<PositionData> positions = await positionsTask;
        List<CompanyData> companies = await companiesTask;
        List<ImageInfo> imagesInfos = await imagesTask;

        department = departments?.FirstOrDefault(d => d.ProjectsIds != null && d.ProjectsIds.Contains(dbProject.Id));

        //rework
        List<DbProjectUser> projectUsersForCount = await _userRepository.GetAsync(usersIds);

        usersInfo = dbProject.Users
          .Select(pu =>
          {
            UserData mappedUser = usersDatas.FirstOrDefault(x => x.Id == pu.UserId);

            return _projectUserInfoMapper.Map(
              mappedUser,
              imagesInfos?.FirstOrDefault(i => i.Id == mappedUser.ImageId),
              positions?.FirstOrDefault(p => p.Users.Any(user => user.UserId == pu.UserId)),
              companies?.FirstOrDefault(c => c.Users.Any(user => user.UserId == pu.UserId)),
              departments?.FirstOrDefault(d => d.UsersIds.Any(id => id == pu.UserId)),
              pu,
              projectUsersForCount.Where(u => u.UserId == pu.UserId).Count());
          })
          .ToList();
      }
      else
      {
        department = (await GetDepartmentAsync(dbProject.Id, null, response.Errors))?.FirstOrDefault();
      }

      List<Guid> files = dbProject.Files.Select(x => x.FileId).ToList();
      List<ImageInfo> imagesinfo = await GetProjectImagesAsync(dbProject.Images.Select(x => x.ImageId).ToList(), response.Errors);

      response.Status = response.Errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;
      response.Body = _projectResponseMapper.Map(dbProject, usersInfo, files, imagesinfo, _departmentInfoMapper.Map(department));

      return response;
    }
  }
}
