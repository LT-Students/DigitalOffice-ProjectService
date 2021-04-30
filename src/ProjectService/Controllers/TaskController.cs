using System;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromQuery] Guid id,
            [FromBody] JsonPatchDocument<EditTaskRequest> request,
            IEditTaskCommand command)
        {
            return command.Execute(id, request);
        }
    }
}
