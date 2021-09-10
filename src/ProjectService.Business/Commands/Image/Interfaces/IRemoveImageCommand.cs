using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces
{
    [AutoInject]
    public interface IRemoveImageCommand
    {
        OperationResultResponse<bool> Execute(List<RemoveImageRequest> request);
    }
}
