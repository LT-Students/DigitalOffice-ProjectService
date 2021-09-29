using System.Collections.Generic;
using System.Linq;
using System.Net;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
  public class FindTaskPropertyCommand : IFindTaskPropertyCommand
  {
    private readonly ITaskPropertyInfoMapper _mapper;
    private readonly ITaskPropertyRepository _repository;
    private readonly IBaseFindRequestValidator _findRequestValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FindTaskPropertyCommand(
      ITaskPropertyRepository repository,
      ITaskPropertyInfoMapper mapper,
      IBaseFindRequestValidator findRequestValidator,
      IHttpContextAccessor httpContextAccessor)
    {
      _mapper = mapper;
      _repository = repository;
      _findRequestValidator = findRequestValidator;
      _httpContextAccessor = httpContextAccessor;
    }

    public FindResultResponse<TaskPropertyInfo> Execute(FindTaskPropertiesFilter filter)
    {
      if (_findRequestValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new FindResultResponse<TaskPropertyInfo>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      return new FindResultResponse<TaskPropertyInfo>
      {
        Body = _repository
          .Find(filter, out int totalCount)
          .Select(tp => _mapper.Map(tp))
          .ToList(),
        Status = OperationResultStatusType.FullSuccess,
        TotalCount = totalCount
      };
    }
  }
}
