using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    [AutoInject]
    public interface IGetTaskCommand
    {
        public TaskResponse Execute(Guid taskId);
    }
}
