﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  public class CreateFileCommand : ICreateFileCommand
  {
    private readonly IDbProjectFileMapper _mapper;
    private readonly IFileRepository _repository;
    private readonly IRequestClient<ICreateFilesRequest> _rcFiles;
    private readonly ILogger<CreateFileCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    private async Task<bool> CreateFileAsync(List<FileData> files, List<string> errors)
    {
      if (files == null || !files.Any())
      {
        return false;
      }

      string logMessage = "Errors while creating files. Errors: {Errors}";

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcFiles.GetResponse<IOperationResult<bool>>(
            ICreateFilesRequest.CreateObj(
              files,
              _httpContextAccessor.HttpContext.GetUserId()));

        if (response.Message.IsSuccess)
        {
          return true;
        }

        _logger.LogWarning(
          logMessage,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      errors.Add("Can not create files. Please try again later.");

      return false;
    }

    public CreateFileCommand(
      IDbProjectFileMapper mapper,
      IFileRepository repository,
      IRequestClient<ICreateFilesRequest> rcFiles,
      ILogger<CreateFileCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository)
    {
      _mapper = mapper;
      _repository = repository;
      _rcFiles = rcFiles;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
    }

    public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateFilesRequest request)
    {
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(await _userRepository.DoesExistAsync(request.ProjectId, userId, true)))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<List<Guid>>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string> { "Not enough rights." }
        };
      }

      OperationResultResponse<List<Guid>> response = new();

      List<FileData> files = request.Files.Select(x =>
        new FileData(Guid.NewGuid(),
          x.Name,
          x.Content,
          x.Extension)).ToList();

      await CreateFileAsync(files, response.Errors);

      if (response.Errors.Any())
      {
        response.Status = OperationResultStatusType.Failed;
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return response;
      }

      response.Body = await _repository.CreateAsync(files.Select(x =>
        _mapper.Map(x.Id, request.ProjectId)).ToList());

      response.Status = OperationResultStatusType.FullSuccess;
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
