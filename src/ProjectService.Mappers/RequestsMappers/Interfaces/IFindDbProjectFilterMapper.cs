using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
    public interface IFindDbProjectFilterMapper
    {
        FindDbProjectsFilter Map(FindProjectsFilter projectsFilter, IDictionary<Guid, string> idNameDeaprtments);
    }
}
