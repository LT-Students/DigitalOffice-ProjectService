using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    [AutoInject]
    public interface IFindProjectsCommand
    {
        ProjectsResponse Execute(FindProjectsFilter filter, int skipCount, int takeCount);
    }
}
