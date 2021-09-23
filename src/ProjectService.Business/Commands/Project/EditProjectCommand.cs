using System;
using System.Collections.Generic;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class EditProjectCommand : IEditProjectCommand
  {
    private readonly IEditProjectValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IPatchDbProjectMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public EditProjectCommand(
      IEditProjectValidator validator,
      IAccessValidator accessValidator,
      IPatchDbProjectMapper mapper,
      IProjectRepository projectRepository,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository)
    {
      _validator = validator;
      _accessValidator = accessValidator;
      _mapper = mapper;
      _projectRepository = projectRepository;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
    }

    public OperationResultResponse<bool> Execute(Guid projectId, JsonPatchDocument<EditProjectRequest> request)
    {
      OperationResultResponse<bool> response = new();

      Guid userId = _httpContextAccessor.HttpContext.GetUserId();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveProjects)
        && !_userRepository.AreUserProjectExist(userId, projectId, true))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      response.Body = _projectRepository.Edit(projectId, _mapper.Map(request));
      if (!response.Body)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Project can not be edit.");

        return response;
      }

      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
