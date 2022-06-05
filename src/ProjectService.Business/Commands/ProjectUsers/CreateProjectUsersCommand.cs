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
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Time;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class CreateProjectUsersCommand : ICreateProjectUsersCommand
  {
    private readonly IProjectUserRepository _repository;
    private readonly IDbProjectUserMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IProjectUsersRequestValidator _validator;
    private readonly IBus _bus;
    private readonly IResponseCreator _responseCreator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public CreateProjectUsersCommand(
      IProjectUserRepository repository,
      IDbProjectUserMapper mapper,
      IAccessValidator accessValidator,
      IProjectUsersRequestValidator validator,
      IBus bus,
      IResponseCreator responseCreator,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _mapper = mapper;
      _validator = validator;
      _repository = repository;
      _accessValidator = accessValidator;
      _bus = bus;
      _responseCreator = responseCreator;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(ProjectUsersRequest request)
    {
      List<string> errors = new();

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        && !await _repository.IsProjectAdminAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId()))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      List<DbProjectUser> existingUsers = await _repository.GetExistingUsersAsync(request.ProjectId, request.Users.Select(u => u.UserId));

      request.Users = request.Users.ExceptBy(existingUsers.Select(x => x.UserId), request => request.UserId).ToList();

      bool result = await _repository.CreateAsync(_mapper.Map(request), existingUsers);

      if (result)
      {
        await _bus.Publish<ICreateWorkTimePublish>(ICreateWorkTimePublish.CreateObj(
          request.ProjectId,
          request.Users.Select(u => u.UserId).ToList()));

        await _globalCache.RemoveAsync(request.ProjectId);
      }

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result,
        Errors = errors
      };
    }
  }
}
