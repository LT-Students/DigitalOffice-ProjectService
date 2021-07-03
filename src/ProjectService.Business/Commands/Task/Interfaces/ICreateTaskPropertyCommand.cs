using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    [AutoInject]
    public interface ICreateTaskPropertyCommand
    {
        OperationResultResponse<IEnumerable<Guid>> Execute(CreateTaskPropertyRequest request);
    }
}
