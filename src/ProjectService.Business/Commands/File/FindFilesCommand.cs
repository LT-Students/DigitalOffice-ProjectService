using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File
{
  public class FindFilesCommand : IFindFilesCommand
  {
    private readonly IFileRepository _repository;
    private readonly IResponseCreator _responseCreator;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IFileService _fileService;
    private readonly IBaseFindFilterValidator _findFilterValidator;

    public FindFilesCommand(
      IFileRepository repository,
      IFileAccessMapper accessMapper,
      IResponseCreator responseCreator,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IProjectUserRepository projectUserRepository,
      IFileService fileService,
      IBaseFindFilterValidator findFilterValidator)
    {
      _repository = repository;
      _responseCreator = responseCreator;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _projectUserRepository = projectUserRepository;
      _fileService = fileService;
      _findFilterValidator = findFilterValidator;
    }

    public async Task<FindResultResponse<FileCharacteristicsData>> ExecuteAsync(Guid projectId, BaseFindFilter findFilter)
    {
      if (!_findFilterValidator.ValidateCustom(findFilter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<FileCharacteristicsData>(HttpStatusCode.BadRequest, errors);
      }

      FileAccessType accessType = FileAccessType.Public;

      DbProjectUser user = (await _projectUserRepository.GetAsync(new List<Guid> { _httpContextAccessor.HttpContext.GetUserId() })).FirstOrDefault();
      if (await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        || user?.Role == (int)ProjectUserRoleType.Manager)
      {
        accessType = FileAccessType.Manager;
      }
      else if (user is not null)
      {
        accessType = FileAccessType.Team;
      }

      (List<DbProjectFile> dbProjects, int totalCount) = await _repository.FindAsync(projectId, findFilter, accessType);

      if (dbProjects is null)
      {
        return _responseCreator.CreateFailureFindResponse<FileCharacteristicsData>(HttpStatusCode.NotFound);
      }

      List<FileCharacteristicsData> files = await _fileService.GetFilesAsync(dbProjects.Select(file => file.Id).ToList(), errors);

      return errors.Any()
        ? _responseCreator.CreateFailureFindResponse<FileCharacteristicsData>(HttpStatusCode.BadRequest, errors)
        : new FindResultResponse<FileCharacteristicsData>(
          body: files,
          totalCount: totalCount);
    }
  }
}
