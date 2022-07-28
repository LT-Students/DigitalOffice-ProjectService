using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.File;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IFileService
  {
    Task<List<FileCharacteristicsData>> GetFilesAsync(List<Guid> filesIds, List<string> errors = null);
  }
}
