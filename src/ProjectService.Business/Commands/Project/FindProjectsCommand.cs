using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class FindProjectsCommand : IFindProjectsCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IBaseFindFilterValidator _findFilterValidator;
    private readonly IProjectInfoMapper _mapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly IDepartmentService _departmentService;
    private readonly IResponseCreator _responseCreator;

    public FindProjectsCommand(
      IProjectRepository repository,
      IBaseFindFilterValidator findFilterValidator,
      IProjectInfoMapper mapper,
      IDepartmentInfoMapper departmentMapper,
      IDepartmentService departmentService,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _findFilterValidator = findFilterValidator;
      _mapper = mapper;
      _departmentMapper = departmentMapper;
      _departmentService = departmentService;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<ProjectInfo>> ExecuteAsync(FindProjectsFilter filter)
    {
      if (!_findFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<ProjectInfo>(HttpStatusCode.BadRequest, errors);
      }

      (List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount) = await _repository.FindAsync(filter);

      List<DepartmentData> departments = filter.IncludeDepartment
        ? await _departmentService.GetDepartmentsAsync(
          projectsIds: dbProjects.Select(p => p.dbProject.Id).ToList(),
          errors: errors)
        : default;

      return new FindResultResponse<ProjectInfo>(
      body: dbProjects.Select(p => _mapper.Map(p.dbProject, p.usersCount, _departmentMapper.Map(departments?.FirstOrDefault(d => d.ProjectsIds.Contains(p.dbProject.Id))))).ToList(),
      totalCount: totalCount);
    }
  }
}
