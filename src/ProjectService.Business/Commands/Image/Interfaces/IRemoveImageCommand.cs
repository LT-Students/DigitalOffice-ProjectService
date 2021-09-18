using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface IRemoveImageCommand
  {
    OperationResultResponse<bool> Execute(List<Guid> request);
  }
}
