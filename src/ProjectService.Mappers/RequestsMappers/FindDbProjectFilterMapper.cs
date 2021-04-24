using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class FindDbProjectFilterMapper : IFindDbProjectFilterMapper
    {
        public FindDbProjectsFilter Map(FindProjectsFilter projectsFilter, IDictionary<Guid, string> idNameDeaprtments)
        {
            if (projectsFilter == null)
            {
                throw new ArgumentNullException(nameof(projectsFilter));
            }

            return new FindDbProjectsFilter
            {
                Name = projectsFilter.Name,
                ShortName = projectsFilter.ShortName,
                IdNameDepartments = idNameDeaprtments
            };
        }
    }
}
