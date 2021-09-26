using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces
{
    [AutoInject]
    public interface IGetTaskCommand
    {
        OperationResultResponse<TaskResponse> Execute(Guid taskId, bool isFullModel=true);
    }
}
