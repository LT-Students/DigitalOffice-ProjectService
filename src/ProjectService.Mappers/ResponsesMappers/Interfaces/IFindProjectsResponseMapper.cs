using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    [AutoInject]
    public interface IFindProjectsResponseMapper
    {
        public ProjectsResponse Map(List<DbProject> dbProject, int totalCount, string departmentName, List<string> errors);
    }
}
