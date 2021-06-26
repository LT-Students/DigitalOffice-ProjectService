using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces
{
    public interface IFindTaskPropertyCommand
    {
        FindResponse<TaskPropertyInfo> Execute(Guid? projectId, string name, int skipCount, int takeCount);
    }
}
