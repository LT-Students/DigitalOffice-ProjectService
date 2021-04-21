using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class FindDbProjectFilterMapper : IFindDbProjectFilterMapper
    {
        public FindDbProjectsFilter Map(FindProjectsFilter projectsFilter, List<Guid> departmentIds)
        {
            if (projectsFilter == null)
            {
                throw new ArgumentNullException(nameof(projectsFilter));
            }

            return new FindDbProjectsFilter
            {
                Name = projectsFilter.Name,
                ShortName = projectsFilter.ShortName,
                DepartmentIds = departmentIds
            };
        }
    }
}
