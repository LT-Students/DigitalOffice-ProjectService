﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.File;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  public class CreateFilesCommand : ICreateFilesCommand
  {
    private readonly IDbProjectFileMapper _mapper;
    private readonly IFileRepository _repository;
    private readonly IRequestClient<ICreateFilesPublish> _rcFiles;
    private readonly ILogger<CreateFilesCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IFileDataMapper _fileDataMapper;
    private readonly IFileService _fileService;

    public CreateFilesCommand(
      IDbProjectFileMapper mapper,
      IFileRepository repository,
      IRequestClient<ICreateFilesPublish> rcFiles,
      ILogger<CreateFilesCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IProjectUserRepository userRepository,
      IResponseCreator responseCreator,
      IFileDataMapper fileDataMapper,
      IFileService fileService)
    {
      _mapper = mapper;
      _repository = repository;
      _rcFiles = rcFiles;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
      _fileDataMapper = fileDataMapper;
      _fileService = fileService;
    }

    public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateFilesRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _userRepository.DoesExistAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId(), isManager: true))
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<List<Guid>> response = new();

      List<FileAccess> accesses = new List<FileAccess>();
      List<FileData> files = request.Files.Select(x => _fileDataMapper.Map(x, accesses)).ToList();

      await _fileService.CreateFilesAsync(files, response.Errors);

      if (response.Errors.Any())
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(HttpStatusCode.BadRequest, response.Errors);
      }

      response.Body = await _repository.CreateAsync(accesses.Select(x =>
        _mapper.Map(x.FileId, request.ProjectId, x.Access)).ToList());

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
