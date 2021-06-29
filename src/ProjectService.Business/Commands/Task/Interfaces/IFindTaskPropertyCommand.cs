using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    [AutoInject]
    public interface IFindTaskPropertyCommand
    {
        FindResponse<TaskPropertyInfo> Execute(FindTaskPropertiesFilter filter, int skipCount, int takeCount);
    }
}
