using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces
{
    [AutoInject]
    public interface IFindProjectsResponseMapper
    {
        public FindResponse<ProjectInfo> Map(List<DbProject> dbProject, int totalCount, List<DepartmentData> departments, List<string> errors);
    }
}
