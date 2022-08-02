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
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class CreateProjectUsersCommand : ICreateProjectUsersCommand
  {
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IDbProjectUserMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateProjectUsersRequestValidator _validator;
    private readonly IResponseCreator _responseCreator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IPublish _publish;

    public CreateProjectUsersCommand(
      IProjectUserRepository projectUserRepository,
      IProjectRepository projectRepository,
      IDbProjectUserMapper mapper,
      IAccessValidator accessValidator,
      ICreateProjectUsersRequestValidator validator,
      IResponseCreator responseCreator,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache,
      IPublish publish)
    {
      _projectUserRepository = projectUserRepository;
      _projectRepository = projectRepository;
      _mapper = mapper;
      _validator = validator;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
      _publish = publish;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(CreateProjectUsersRequest request)
    {
      List<string> errors = new();

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _projectUserRepository.DoesExistAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId(), isManager: true))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      List<DbProjectUser> existingUsers = await _projectUserRepository.GetExistingUsersAsync(request.ProjectId, request.Users.Select(u => u.UserId));
      List<UserRequest> newUsers =  request.Users.ExceptBy(existingUsers.Select(x => x.UserId), request => request.UserId).ToList();

      bool result = await _projectUserRepository.CreateAsync(_mapper.Map(request.ProjectId, newUsers));
      await _projectUserRepository.EditIsActiveAsync(existingUsers, _httpContextAccessor.HttpContext.GetUserId());

      if (result)
      {
        int projectStatus = (await _projectRepository.GetAsync(new GetProjectFilter { ProjectId = request.ProjectId })).dbProject.Status;
        if (projectStatus == (int)ProjectStatusType.Active)
        {
          await _publish.CreateWorkTimeAsync(
            request.ProjectId,
            newUsers.Select(u => u.UserId).ToList());
        }

        await _globalCache.RemoveAsync(request.ProjectId);
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }

      return new()
      {
        Body = result,
        Errors = errors
      };
    }
  }
}
