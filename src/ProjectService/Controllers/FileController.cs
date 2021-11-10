using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class FileController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<List<Guid>>> CreateAsync(
      [FromServices] ICreateFilesCommand command,
      [FromBody] CreateFilesRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveFilesCommand command,
      [FromBody] RemoveFilesRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
