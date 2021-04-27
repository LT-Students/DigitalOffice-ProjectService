using System;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Microsoft.AspNetCore.Components.Route("[controller]")]
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
