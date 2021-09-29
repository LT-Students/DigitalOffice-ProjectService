using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
  public class CreateTaskPropertyCommand : ICreateTaskPropertyCommand
  {
    private readonly IDbTaskPropertyMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IUserRepository _userRepository;
    private readonly ICreateTaskPropertyValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITaskPropertyRepository _taskPropertyRepository;

    public CreateTaskPropertyCommand(
      IDbTaskPropertyMapper mapper,
      IAccessValidator accessValidator,
      ITaskPropertyRepository taskPropertyRepository,
      IUserRepository userRepository,
      ICreateTaskPropertyValidator validator,
      IHttpContextAccessor httpContextAccessor)
    {
      _mapper = mapper;
      _validator = validator;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _taskPropertyRepository = taskPropertyRepository;
      _httpContextAccessor = httpContextAccessor;
    }

    public OperationResultResponse<IEnumerable<Guid>> Execute(CreateTaskPropertyRequest request)
    {
      if (!(_accessValidator.IsAdmin()
        || _userRepository.AreUserProjectExist(_httpContextAccessor.HttpContext.GetUserId(), request.ProjectId)))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<IEnumerable<Guid>>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new List<string> { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<IEnumerable<Guid>>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      List<DbTaskProperty> dbTaskProperties = request.TaskProperties.Select(x => _mapper.Map(x, userId, request.ProjectId)).ToList();

      _taskPropertyRepository.Create(dbTaskProperties);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<IEnumerable<Guid>>
      {
        Body = dbTaskProperties.Select(x => x.Id),
        Status = OperationResultStatusType.FullSuccess
      };
    }
  }
}
