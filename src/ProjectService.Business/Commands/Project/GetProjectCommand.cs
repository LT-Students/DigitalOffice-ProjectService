using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class GetProjectCommand : IGetProjectCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IProjectUserRepository _userRepository;
    private readonly IProjectResponseMapper _projectResponseMapper;
    private readonly IUserInfoMapper _projectUserInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IFileAccessMapper _accessMapper;
    private readonly IResponseCreator _responseCreator;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDepartmentService _departmentService;
    private readonly IImageService _imageService;
    private readonly IUserService _userService;
    private readonly IPositionService _positionService;
    private readonly ICompanyService _companyService;

    public GetProjectCommand(
      IProjectRepository repository,
      IProjectUserRepository userRepository,
      IProjectResponseMapper projectResponsMapper,
      IUserInfoMapper projectUserInfoMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      IFileAccessMapper accessMapper,
      IResponseCreator responseCreator,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDepartmentService departmentService,
      IImageService imageService,
      IUserService userService,
      IPositionService positionService,
      ICompanyService companyService)
    {
      _repository = repository;
      _userRepository = userRepository;
      _projectResponseMapper = projectResponsMapper;
      _projectUserInfoMapper = projectUserInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _accessMapper = accessMapper;
      _responseCreator = responseCreator;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _departmentService = departmentService;
      _imageService = imageService;
      _userService = userService;
      _positionService = positionService;
      _companyService = companyService;
    }

    public async Task<OperationResultResponse<ProjectResponse>> ExecuteAsync(GetProjectFilter filter)
    {
      (DbProject dbProject, int usersCount) = await _repository.GetAsync(filter);

      if (dbProject is null)
      {
        return _responseCreator.CreateFailureResponse<ProjectResponse>(System.Net.HttpStatusCode.NotFound);
      }

      OperationResultResponse<ProjectResponse> response = new();
      DepartmentData department = null;
      List<UserInfo> usersInfo = null;

      List<UserData> usersDatas = await _userService.GetUsersDatasAsync(dbProject.Users, response.Errors);
      List<Guid> usersIds = dbProject.Users.Select(u => u.UserId).Distinct().ToList();

      if (usersDatas is not null && usersDatas.Any())
      {
        var positionsTask = _positionService.GetPositionsAsync(usersIds, response.Errors);
        var companiesTask = _companyService.GetCompaniesAsync(usersIds, response.Errors);
        var departmentsTask = _departmentService.GetDepartmentsAsync(response.Errors, new List<Guid>() { dbProject.Id }, usersIds);
        var imagesTask = _imageService.GetImagesAsync(usersDatas.Where(u => u.ImageId.HasValue).Select(u => u.ImageId.Value).ToList(), ImageSource.User, response.Errors);

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
              positions?.FirstOrDefault(p => p.UsersIds.Any(userId => userId == pu.UserId)),
              companies?.FirstOrDefault(c => c.Users.Any(user => user.UserId == pu.UserId)),
              departments?.FirstOrDefault(d => d.Users.FirstOrDefault(user => user.UserId == pu.UserId) != null),
              pu,
              projectUsersForCount.Where(u => u.UserId == pu.UserId).Count());
          })
          .ToList();
      }
      else
      {
        department = (await _departmentService.GetDepartmentsAsync(errors: response.Errors, projectsIds: new List<Guid>() { dbProject.Id }))?.FirstOrDefault();
      }

      AccessType accessType = AccessType.Public;

      DbProjectUser dbProjectUser = (await _userRepository.GetAsync(new List<Guid>() { _httpContextAccessor.HttpContext.GetUserId() }))
        ?.FirstOrDefault();

      if (await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        || dbProjectUser?.Role == (int)ProjectUserRoleType.Manager)
      {
        accessType = AccessType.Manager;
      }
      else if (dbProjectUser is not null)
      {
        accessType = AccessType.Team;
      }

      List<FileAccess> files = dbProject.Files.Where(x => x.Access >= (int)accessType).Select(_accessMapper.Map).ToList();
      List<ImageInfo> imagesinfo = await _imageService.GetImagesAsync(dbProject.Images.Select(x => x.ImageId).ToList(), ImageSource.Project, response.Errors);

      response.Body = _projectResponseMapper.Map(dbProject, usersCount, usersInfo, files, imagesinfo, _departmentInfoMapper.Map(department));

      return response;
    }
  }
}
