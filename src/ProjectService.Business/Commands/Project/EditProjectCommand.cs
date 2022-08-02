using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class EditProjectCommand : IEditProjectCommand
  {
    private readonly IEditProjectRequestValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IPatchDbProjectMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IPublish _publish;

    public EditProjectCommand(
      IEditProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IPatchDbProjectMapper mapper,
      IProjectRepository projectRepository,
      IHttpContextAccessor httpContextAccessor,
      IProjectUserRepository userRepository,
      IResponseCreator responseCreator,
      IGlobalCacheRepository globalCache,
      IPublish publish)
    {
      _validator = validator;
      _accessValidator = accessValidator;
      _mapper = mapper;
      _projectRepository = projectRepository;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
      _globalCache = globalCache;
      _publish = publish;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid projectId,
      JsonPatchDocument<EditProjectRequest> request)
    {
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _userRepository.DoesExistAsync(userId, projectId, true))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      int previousStatus = (await _projectRepository.GetAsync(new GetProjectFilter { ProjectId = projectId })).dbProject
        .Status;

      OperationResultResponse<bool> response = new();
      response.Body = await _projectRepository.EditAsync(projectId, _mapper.Map(request));

      if (!response.Body)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }
      else
      {
        await _globalCache.RemoveAsync(projectId);

        (DbProject dbProject, IEnumerable<Guid> usersIds, _) =
          await _projectRepository.GetAsync(new GetProjectFilter { ProjectId = projectId });
        List<Guid> usersIdsList = usersIds.ToList();

        if (previousStatus != (int)ProjectStatusType.Active
          && dbProject.Status == (int)ProjectStatusType.Active
          && usersIdsList.Any())
        {
          await _publish.CreateWorkTimeAsync(
            projectId,
            usersIdsList);
        }
      }

      return response;
    }
  }
}
