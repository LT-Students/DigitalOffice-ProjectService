using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.File;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IFileService
  {
    Task<bool> CreateFilesAsync(List<FileData> files, List<string> errors);
  }
}
