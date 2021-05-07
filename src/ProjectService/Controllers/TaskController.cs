using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [HttpPost("Create")]
        public OperationResultResponse<Guid> Create(
           [FromServices] ICreateTaskCommand command,
           [FromBody] CreateTaskRequest request)
        {
            return command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromQuery] Guid id,
            [FromBody] JsonPatchDocument<EditTaskRequest> request,
            [FromServices] IEditTaskCommand command)
        {
            return command.Execute(id, request);
        }
    }
}