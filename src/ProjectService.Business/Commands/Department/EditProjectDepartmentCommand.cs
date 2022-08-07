using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Department.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Department;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Department
{
  public class EditProjectDepartmentCommand : IEditProjectDepartmentCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IDbProjectDepartmentMapper _mapper;
    private readonly IProjectDepartmentRepository _departmentRepository;
    private readonly IResponseCreator _responseCreator;

    public EditProjectDepartmentCommand(
      IAccessValidator accessValidator,
      IDbProjectDepartmentMapper mapper,
      IProjectDepartmentRepository projectRepository,
      IResponseCreator responseCreator)
    {
      _accessValidator = accessValidator;
      _mapper = mapper;
      _departmentRepository = projectRepository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditProjectDepartmentRequest request)
    {
      //TODO add check if user is DD
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveDepartments))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (!await _departmentRepository.EditAsync(request.ProjectId, request.DepartmentId))
      {
        await _departmentRepository.CreateAsync(
          _mapper.Map(request.ProjectId, request.DepartmentId.Value));
      }

      return new()
      {
        Body = true
      };
    }
  }
}
