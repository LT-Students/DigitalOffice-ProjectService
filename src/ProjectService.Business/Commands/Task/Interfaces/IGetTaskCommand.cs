using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces
{
    [AutoInject]
    public interface IGetTaskCommand
    {
        Task<OperationResultResponse<TaskResponse>> Execute(Guid taskId, bool isFullModel=true);
    }
}
