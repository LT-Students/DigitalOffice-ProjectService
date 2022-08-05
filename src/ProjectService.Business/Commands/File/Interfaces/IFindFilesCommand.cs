using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.File;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  [AutoInject]
  public interface IFindFilesCommand
  {
    Task<FindResultResponse<FileInfo>> ExecuteAsync(FindProjectFilesFilter findFilter);
  }
}
