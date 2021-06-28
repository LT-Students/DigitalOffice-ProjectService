using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    public interface IFindTaskPropertyCommand
    {
        FindResponse<TaskPropertyInfo> Execute(FindTaskPropertiesFilter filter, int skipCount, int takeCount);
    }
}
