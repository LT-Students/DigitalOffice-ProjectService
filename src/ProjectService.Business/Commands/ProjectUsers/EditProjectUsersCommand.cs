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
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.ProjectUser.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class EditProjectUsersCommand : IEditProjectUsersCommand
  {
    private readonly IProjectUserRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IProjectUsersRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditProjectUsersCommand(
      IProjectUserRepository repository,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IProjectUsersRequestValidator validator,
      IHttpContextAccessor httpContextAccessor)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(ProjectUsersRequest request)
    {
      List<string> errors = new();

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects) && 
          !await _repository.IsProjectAdminAsync(request.ProjectId, _httpContextAccessor.HttpContext.GetUserId()))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      bool result = await _repository.EditProjectUsers(request);

      return new OperationResultResponse<bool>
      {
        Status = errors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess, 
        Body = result,
        Errors = errors
      };
    }
  }
}
