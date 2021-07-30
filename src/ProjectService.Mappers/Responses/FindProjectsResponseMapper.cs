using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses
{
    public class FindProjectsResponseMapper : IFindProjectsResponseMapper
    {
        private readonly IProjectInfoMapper _mapper;
        public FindProjectsResponseMapper(
            IProjectInfoMapper mapper)
        {
            _mapper = mapper;
        }

        public FindResponse<ProjectInfo> Map(List<DbProject> dbProjects, int totalCount, IDictionary<Guid, string> departmentsNames, List<string> errors)
        {
            if (dbProjects == null)
            {
                throw new ArgumentNullException(nameof(dbProjects));
            }

            return new FindResponse<ProjectInfo>
            {
                TotalCount = totalCount,
                Body = dbProjects.Select(p =>
                {
                    string departmentName = null;
                    if (p.DepartmentId.HasValue)
                    {
                        departmentsNames.TryGetValue(p.DepartmentId.Value, out departmentName);
                    }
                    return _mapper.Map(p, departmentName);
                }),
                Errors = errors
            };
        }
    }
}
