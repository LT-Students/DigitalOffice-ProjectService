using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ImageController : ControllerBase
  {
    [HttpPost("create")]
    public OperationResultResponse<List<Guid>> Create(
      [FromServices] ICreateImageCommand command,
      [FromBody] CreateImageRequest request)
    {
      return command.Execute(request);
    }

    [HttpDelete("remove")]
    public OperationResultResponse<bool> Remove(
      [FromServices] IRemoveImageCommand command,
      [FromBody] List<Guid> request)
    {
      return command.Execute(request);
    }
  }
}
