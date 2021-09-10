using LT.DigitalOffice.Models.Broker.Models.Company;
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
        private readonly IDepartmentInfoMapper _departmentInfoMapper;

        public FindProjectsResponseMapper(
            IProjectInfoMapper mapper,
            IDepartmentInfoMapper departmentInfoMapper)
        {
            _mapper = mapper;
            _departmentInfoMapper = departmentInfoMapper;
        }

        public FindResponse<ProjectInfo> Map(List<DbProject> dbProjects, int totalCount, List<DepartmentData> departments, List<string> errors)
        {
            if (dbProjects == null)
            {
                throw new ArgumentNullException(nameof(dbProjects));
            }

            List<DepartmentInfo> departmentsInfos = departments.Select(_departmentInfoMapper.Map).ToList();

            return new FindResponse<ProjectInfo>
            {
                TotalCount = totalCount,
                Body = dbProjects.Select(p =>
                {
                    return _mapper.Map(p, departmentsInfos.FirstOrDefault(d => p.DepartmentId == d.Id));
                }),
                Errors = errors
            };
        }
    }
}
