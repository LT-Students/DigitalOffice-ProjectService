using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.File;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File
{
  public class RemoveFilesCommand : IRemoveFilesCommand
  {
    private readonly IProjectFileRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IPublish _publish;

    public RemoveFilesCommand(
      IProjectFileRepository repository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IProjectUserRepository userRepository,
      IResponseCreator responseCreator,
      IPublish publish)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
      _publish = publish;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveFilesRequest request)
    {
      if (!await _userRepository.DoesExistAsync(userId: _httpContextAccessor.HttpContext.GetUserId(), projectId: request.ProjectId, isManager: true) 
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new (body: await _repository.RemoveAsync(request.FilesIds));

      if (response.Body)
      {
        await _publish.RemoveFilesAsync(request.FilesIds);
      }
      else
      {
        response = _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, response.Errors);
      }

      return response;
    }
  }
}
