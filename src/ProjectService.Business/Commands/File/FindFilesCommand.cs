﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
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

    public async Task<FindResultResponse<FileCharacteristicsData>> ExecuteAsync(FindProjectFilesFilter findFilter)
    {
      if (!_findFilterValidator.ValidateCustom(findFilter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<FileCharacteristicsData>(HttpStatusCode.BadRequest, errors);
      }

      FileAccessType accessType = FileAccessType.Public;

      ProjectUserRoleType? userRole = await _projectUserRepository.GetUserRoleAsync(findFilter.ProjectId, _httpContextAccessor.HttpContext.GetUserId());
      if (userRole == ProjectUserRoleType.Manager
        || await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        accessType = FileAccessType.Manager;
      }
      else if (userRole.HasValue)
      {
        accessType = FileAccessType.Team;
      }

      (List<DbProjectFile> dbFiles, int totalCount) = await _repository.FindAsync(findFilter, accessType);

      List<FileCharacteristicsData> files = await _fileService.GetFilesCharacteristicsAsync(dbFiles?.Select(file => file.FileId).ToList(), errors);

      return new FindResultResponse<FileCharacteristicsData>(
        body: files,
        totalCount: totalCount);
    }
  }
}
