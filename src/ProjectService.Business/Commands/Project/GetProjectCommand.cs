using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class GetProjectCommand : IGetProjectCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IProjectResponseMapper _projectResponseMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IResponseCreator _responseCreator;
    private readonly IDepartmentService _departmentService;

    public GetProjectCommand(
      IProjectRepository repository,
      IProjectResponseMapper projectResponsMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      IResponseCreator responseCreator,
      IDepartmentService departmentService)
    {
      _repository = repository;
      _projectResponseMapper = projectResponsMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _responseCreator = responseCreator;
      _departmentService = departmentService;
    }

    public async Task<OperationResultResponse<ProjectResponse>> ExecuteAsync(GetProjectFilter filter)
    {
      DbProject dbProject = await _repository.GetAsync(filter);

      if (dbProject is null)
      {
        return _responseCreator.CreateFailureResponse<ProjectResponse>(HttpStatusCode.NotFound);
      }

      OperationResultResponse<ProjectResponse> response = new();

      DepartmentData department = filter.IncludeDepartment
        ? (await _departmentService
          .GetDepartmentsAsync(errors: response.Errors, projectsIds: new List<Guid>() { dbProject.Id }))?.FirstOrDefault()
        : null;

      response.Body = _projectResponseMapper.Map(dbProject, _departmentInfoMapper.Map(department));

      return response;
    }
  }
}
