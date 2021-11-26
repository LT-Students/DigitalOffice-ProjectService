using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
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
    private readonly IUserRepository _userRepository;
    private readonly IResponseCreater _responseCreator;
    private readonly ICacheNotebook _cacheNotebook;

    public EditProjectCommand(
      IEditProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IPatchDbProjectMapper mapper,
      IProjectRepository projectRepository,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IResponseCreater responseCreator,
      ICacheNotebook cacheNotebook)
    {
      _validator = validator;
      _accessValidator = accessValidator;
      _mapper = mapper;
      _projectRepository = projectRepository;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
      _cacheNotebook = cacheNotebook;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid projectId, JsonPatchDocument<EditProjectRequest> request)
    {
      OperationResultResponse<bool> response = new();

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

      response.Body = await _projectRepository.EditAsync(projectId, _mapper.Map(request));

      response.Status = OperationResultStatusType.FullSuccess;
      if (!response.Body)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Project can not be edit.");
      }
      else
      {
        await _cacheNotebook.RemoveAsync(projectId);
      }

      return response;
    }
  }
}
