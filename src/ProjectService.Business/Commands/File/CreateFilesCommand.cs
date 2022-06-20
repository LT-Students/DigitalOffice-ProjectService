using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;


namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  public class CreateFilesCommand : ICreateFilesCommand
  {
    private readonly IDbProjectFileMapper _mapper;
    private readonly IFileRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IFileDataMapper _fileDataMapper;
    private readonly IPublish _publish;
   
    public CreateFilesCommand(
      IDbProjectFileMapper mapper,
      IFileRepository repository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IProjectUserRepository userRepository,
      IResponseCreator responseCreator,
      IFileDataMapper fileDataMapper,
      IPublish publish)
    {
      _mapper = mapper;
      _repository = repository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
      _fileDataMapper = fileDataMapper;
      _publish = publish;
    }

    public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateFilesRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _userRepository.DoesExistAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId(), isManager: true))
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(HttpStatusCode.Forbidden);
      }

      List<FileAccess> accesses = new List<FileAccess>();
      List<FileData> files = request.Files.Select(x => _fileDataMapper.Map(x, accesses)).ToList();

      OperationResultResponse<List<Guid>> response = new(body: await _repository.
        CreateAsync(accesses.Select(x => _mapper.Map(x.FileId, request.ProjectId, x.Access)).ToList()));

      if (response.Body.Any())
      {
        await _publish.CreateFilesAsync(files);
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }

      return response;
    }
  }
}
