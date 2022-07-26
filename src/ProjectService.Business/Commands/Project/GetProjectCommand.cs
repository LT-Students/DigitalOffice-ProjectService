using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
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
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IFileAccessMapper _accessMapper;
    private readonly IResponseCreator _responseCreator;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDepartmentService _departmentService;
    private readonly IImageService _imageService;

    public GetProjectCommand(
      IProjectRepository repository,
      IProjectUserRepository userRepository,
      IProjectResponseMapper projectResponsMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      IFileAccessMapper accessMapper,
      IResponseCreator responseCreator,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDepartmentService departmentService,
      IImageService imageService)
    {
      _repository = repository;
      _userRepository = userRepository;
      _projectResponseMapper = projectResponsMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _accessMapper = accessMapper;
      _responseCreator = responseCreator;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _departmentService = departmentService;
      _imageService = imageService;
    }

    public async Task<OperationResultResponse<ProjectResponse>> ExecuteAsync(GetProjectFilter filter)
    {
      (DbProject dbProject, IEnumerable<Guid> usersIds, int usersCount) = await _repository.GetAsync(filter);

      if (dbProject is null)
      {
        return _responseCreator.CreateFailureResponse<ProjectResponse>(System.Net.HttpStatusCode.NotFound);
      }

      OperationResultResponse<ProjectResponse> response = new();

      DepartmentData department = (await _departmentService.GetDepartmentsAsync(errors: response.Errors, projectsIds: new List<Guid>() { dbProject.Id }))?.FirstOrDefault();

      FileAccessType accessType = FileAccessType.Public;

      DbProjectUser dbProjectUser = (await _userRepository.GetAsync(new List<Guid>() { _httpContextAccessor.HttpContext.GetUserId() }))
        ?.FirstOrDefault();

      if (await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        || dbProjectUser?.Role == (int)ProjectUserRoleType.Manager)
      {
        accessType = FileAccessType.Manager;
      }
      else if (dbProjectUser is not null)
      {
        accessType = FileAccessType.Team;
      }

      List<FileAccess> files = null;
      List<ImageInfo> imagesInfo = null;
      
      if (filter.IncludeFiles)
      {
        files = dbProject.Files.Where(x => x.Access >= (int)accessType).Select(_accessMapper.Map).ToList();
      }

      if (filter.IncludeImages)
      {
        imagesInfo = await _imageService.GetImagesAsync(dbProject.Images.Select(x => x.ImageId).ToList(), ImageSource.Project, response.Errors);
      }

      response.Body = _projectResponseMapper.Map(dbProject, usersCount, usersIds, files, imagesInfo, _departmentInfoMapper.Map(department));

      return response;
    }
  }
}
