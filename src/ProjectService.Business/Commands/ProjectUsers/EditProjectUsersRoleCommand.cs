using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class EditProjectUsersRoleCommand : IEditProjectUsersRoleCommand
  {
    private readonly IProjectUserRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IEditProjectUsersRoleRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public EditProjectUsersRoleCommand(
      IProjectUserRepository repository,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IEditProjectUsersRoleRequestValidator validator,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid projectId, EditProjectUsersRoleRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects) 
        && !await _repository.DoesExistAsync(projectId, _httpContextAccessor.HttpContext.GetUserId(), isManager: true))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync((projectId, request));
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      bool result = await _repository.EditAsync(projectId, request);

      if (result)
      {
        await _globalCache.RemoveAsync(projectId);
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }

      return new OperationResultResponse<bool>
      {
        Body = result
      };
    }
  }
}
