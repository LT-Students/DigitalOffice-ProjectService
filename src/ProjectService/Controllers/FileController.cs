using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.File;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class FileController : ControllerBase
  {
    [HttpGet("find")]
    public async Task<FindResultResponse<FileInfo>> FindAsync(
      [FromServices] IFindFilesCommand command,
      [FromQuery] FindProjectFilesFilter findFilter)
    {
      return await command.ExecuteAsync(findFilter);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveFilesCommand command,
      [FromBody] RemoveFilesRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPut("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditFilesCommand command,
      [FromQuery] Guid fileId,
      [FromQuery] FileAccessType newAccessType)
    {
      return await command.ExecuteAsync(fileId, newAccessType);
    }
  }
}
