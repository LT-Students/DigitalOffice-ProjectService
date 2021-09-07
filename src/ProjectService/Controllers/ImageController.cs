using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost("create")]
        public OperationResultResponse<bool> Create(
            [FromServices] ICreateImageCommand command,
            [FromBody] CreateImageRequest request)
        {
            return command.Execute(request);
        }

        [HttpDelete("remove")]
        public OperationResultResponse<bool> Remove(
            [FromServices] IRemoveImageCommand command,
            [FromBody] List<RemoveImageRequest> request)
        {
            return command.Execute(request);
        }
    }
}
