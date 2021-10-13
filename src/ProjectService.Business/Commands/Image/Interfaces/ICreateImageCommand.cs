using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface ICreateImageCommand
  {
    Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateImageRequest request);
  }
}
