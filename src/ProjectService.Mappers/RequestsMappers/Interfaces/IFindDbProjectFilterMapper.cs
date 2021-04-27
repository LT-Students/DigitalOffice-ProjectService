using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
    public interface IFindDbProjectFilterMapper
    {
        FindDbProjectsFilter Map(FindProjectsFilter projectsFilter, IDictionary<Guid, string> idNameDeaprtments);
    }
}
