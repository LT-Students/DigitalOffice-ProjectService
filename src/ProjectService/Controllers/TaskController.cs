using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [HttpGet("find")]
        public FindResultResponse<TaskInfo> Find(
            [FromServices] IFindTasksCommand command,
            [FromQuery] FindTasksFilter filter)
        {
            return command.Execute(filter);
        }

        [HttpGet("get")]
        public OperationResultResponse<TaskResponse> Get(
            [FromQuery] Guid taskId,
            [FromServices] IGetTaskCommand command)
        {
            return command.Execute(taskId);
        }

        [HttpPost("create")]
        public OperationResultResponse<Guid> Create(
           [FromServices] ICreateTaskCommand command,
           [FromBody] CreateTaskRequest request)
        {
            return command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromQuery] Guid taskId,
            [FromBody] JsonPatchDocument<EditTaskRequest> request,
            [FromServices] IEditTaskCommand command)
        {
            return command.Execute(taskId, request);
        }
    }
}
