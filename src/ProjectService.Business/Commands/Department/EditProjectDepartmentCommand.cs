using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Department.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Department;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Department
{
  public class EditProjectDepartmentCommand : IEditProjectDepartmentCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbProjectDepartmentMapper _mapper;
    private readonly IProjectDepartmentRepository _projectDepartmentRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IDepartmentService _departmentService;

    public EditProjectDepartmentCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDbProjectDepartmentMapper mapper,
      IProjectDepartmentRepository projectDepartmentRepository,
      IResponseCreator responseCreator,
      IDepartmentService departmentService)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _projectDepartmentRepository = projectDepartmentRepository;
      _responseCreator = responseCreator;
      _departmentService = departmentService;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditProjectDepartmentRequest request)
    {
      DbProjectDepartment dbProjectDepartment = await _projectDepartmentRepository.GetAsync(request.ProjectId);

      if (dbProjectDepartment is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      if ((!request.DepartmentId.HasValue
          && (await _departmentService.CheckDepartmentUserRoleAsync(
            userId: _httpContextAccessor.HttpContext.GetUserId(),
            departmentId: dbProjectDepartment.DepartmentId) != DepartmentUserRole.Manager)
          && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveDepartments))
        || (request.DepartmentId.HasValue
          && (await _departmentService.CheckDepartmentUserRoleAsync(
            userId: _httpContextAccessor.HttpContext.GetUserId(),
            departmentId: request.DepartmentId.Value) != DepartmentUserRole.Manager)
          && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveDepartments)))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (!await _projectDepartmentRepository.EditAsync(request.ProjectId, request.DepartmentId))
      {
        await _projectDepartmentRepository.CreateAsync(
          _mapper.Map(request.ProjectId, request.DepartmentId.Value));
      }

      return new()
      {
        Body = true
      };
    }
  }
}
