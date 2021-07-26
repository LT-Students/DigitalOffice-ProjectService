using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [HttpGet("find")]
        public FindResponse<TaskInfo> Find(
            [FromServices] IFindTasksCommand command,
            [FromQuery] FindTasksFilter filter,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount)
        {
            return command.Execute(filter, skipCount, takeCount);
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