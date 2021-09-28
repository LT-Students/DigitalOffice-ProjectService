using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses
{
  public class FindProjectsResponseMapper : IFindProjectsResponseMapper
  {
    private readonly IProjectInfoMapper _mapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;

    public FindProjectsResponseMapper(
      IProjectInfoMapper mapper,
      IDepartmentInfoMapper departmentInfoMapper)
    {
      _mapper = mapper;
      _departmentInfoMapper = departmentInfoMapper;
    }

    public FindResultResponse<ProjectInfo> Map(List<DbProject> dbProjects, int totalCount, List<DepartmentData> departments, List<string> errors)
    {
      if (dbProjects == null)
      {
       return null;
      }

      List<DepartmentInfo> departmentsInfos = departments?.Select(_departmentInfoMapper.Map).ToList();

      return new FindResultResponse<ProjectInfo>
      {
        TotalCount = totalCount,
        Body = dbProjects.Select(p =>
        {
          return _mapper.Map(p, departmentsInfos?.FirstOrDefault(d => p.DepartmentId == d.Id));
        }).ToList(),
        Errors = errors
      };
    }
  }
}
