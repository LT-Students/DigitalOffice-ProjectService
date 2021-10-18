using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ImageController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<List<Guid>>> Create(
      [FromServices] ICreateImageCommand command,
      [FromBody] CreateImageRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> Remove(
      [FromServices] IRemoveImageCommand command,
      [FromBody] RemoveImageRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
