using System;
using System.Collections.Generic;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task
{
  public class EditTaskPropertyCommand : IEditTaskPropertyCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IUserRepository _userRepository;
    private readonly ITaskPropertyRepository _taskPropertyRepository;
    private readonly IPatchDbTaskPropertyMapper _mapper;
    private readonly IEditTaskPropertyValidator _validator;

    public EditTaskPropertyCommand(
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IUserRepository userRepository,
      ITaskPropertyRepository taskPropertyRepository,
      IPatchDbTaskPropertyMapper mapper,
      IEditTaskPropertyValidator validator)
    {
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _userRepository = userRepository;
      _taskPropertyRepository = taskPropertyRepository;
      _mapper = mapper;
      _validator = validator;
    }

    public OperationResultResponse<bool> Execute(Guid taskPropertyId, JsonPatchDocument<TaskProperty> patch)
    {
      DbTaskProperty taskProperty = _taskPropertyRepository.Get(taskPropertyId);

      if (taskProperty.ProjectId == null
        || !_userRepository.AreUserProjectExist(_httpContextAccessor.HttpContext.GetUserId(), (Guid)taskProperty.ProjectId)
        || !_accessValidator.HasRights(Rights.AddEditRemoveProjects))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string> { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(patch, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      OperationResultResponse<bool> response = new();

      response.Body = _taskPropertyRepository.Edit(taskProperty, _mapper.Map(patch));

      if (response.Body)
      {
        response.Status = OperationResultStatusType.FullSuccess;
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add($"Can not edit taskProperty with Id: {taskPropertyId}");
      }

      return response;
    }
  }
}
