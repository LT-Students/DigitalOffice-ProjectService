using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  [AutoInject]
  public interface ICreateFileCommand
  {
    Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateFilesRequest request);
  }
}
