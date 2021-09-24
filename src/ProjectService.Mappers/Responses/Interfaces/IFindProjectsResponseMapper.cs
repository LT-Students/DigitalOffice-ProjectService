using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IFindProjectsResponseMapper
  {
    FindResultResponse<ProjectInfo> Map(List<DbProject> dbProject, int totalCount, List<DepartmentData> departments, List<string> errors);
  }
}
