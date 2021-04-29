using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
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

            var projectInfos = new List<ProjectInfo>();
            foreach(var dbProject in dbProjects)
            {
                departmentsNames.TryGetValue(dbProject.DepartmentId, out string departmentName);
                projectInfos.Add(_mapper.Map(dbProject, departmentName));
            }

            return new FindResponse<ProjectInfo>
            {
                TotalCount = totalCount,
                Body = projectInfos,
                Errors = errors
            };
        }
    }
}
