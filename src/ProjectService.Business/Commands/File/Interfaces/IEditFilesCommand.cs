using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  [AutoInject]
  public interface IEditFilesCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid fileId, FileAccessType accessType);
  }
}
