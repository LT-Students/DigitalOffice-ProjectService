using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class RemoveProjectUsersCommand : IRemoveProjectUsersCommand
  {
    private readonly IProjectUserRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public RemoveProjectUsersCommand(
      IProjectUserRepository repository,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid projectId, List<Guid> userIds)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _repository.DoesExistAsync(projectId, _httpContextAccessor.HttpContext.GetUserId(), isManager: true))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (userIds == null || !userIds.Any())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      bool result = await _repository.RemoveAsync(projectId, userIds);

      if (result)
      {
        await _globalCache.RemoveAsync(projectId);
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }

      return new()
      {
        Body = result
      };
    }
  }
}
