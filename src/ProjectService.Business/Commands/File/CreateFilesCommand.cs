using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
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
  public class CreateFilesCommand : ICreateFilesCommand
  {
    private readonly IDbProjectFileMapper _mapper;
    private readonly IFileRepository _repository;
    private readonly IRequestClient<ICreateFilesRequest> _rcFiles;
    private readonly ILogger<CreateFilesCommand> _logger;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IResponseCreater _responseCreator;

    private async Task<bool> CreateFilesAsync(List<FileData> files, List<string> errors)
    {
      if (files == null || !files.Any())
      {
        return false;
      }

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcFiles.GetResponse<IOperationResult<bool>>(
            ICreateFilesRequest.CreateObj(
              files,
              _httpContextAccessor.HttpContext.GetUserId()));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }

        _logger.LogWarning(
          "Errors while creating files. Errors: {Errors}",
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Errors while creating files.");
      }

      errors.Add("Can not create files. Please try again later.");

      return false;
    }

    public CreateFilesCommand(
      IDbProjectFileMapper mapper,
      IFileRepository repository,
      IRequestClient<ICreateFilesRequest> rcFiles,
      ILogger<CreateFilesCommand> logger,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IResponseCreater responseCreator)
    {
      _mapper = mapper;
      _repository = repository;
      _rcFiles = rcFiles;
      _logger = logger;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateFilesRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !(await _userRepository.DoesExistAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId(), true)))
      {
        return _responseCreator.CreateFailureResponse <List<Guid>>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<List<Guid>> response = new();

      List<FileData> files = request.Files.Select(x =>
        new FileData(
          Guid.NewGuid(),
          x.Name,
          x.Content,
          x.Extension)).ToList();

      await CreateFilesAsync(files, response.Errors);

      if (response.Errors.Any())
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(HttpStatusCode.BadRequest, response.Errors);
      }

      response.Body = await _repository.CreateAsync(files.Select(x =>
        _mapper.Map(x.Id, request.ProjectId)).ToList());

      response.Status = OperationResultStatusType.FullSuccess;
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
