using System;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromQuery] Guid id,
            [FromBody] JsonPatchDocument<EditTaskRequest> request,
            [FromServices] IEditTaskCommand command)
        {
            return command.Execute(id, request);
        }

        [HttpGet("get")]
        public TaskResponse Get(
            [FromQuery] Guid taskId,
            [FromQuery] bool isFullModel,
            [FromServices] IGetTaskCommand command)
        {
            return command.Execute(taskId);
        }
    }
}
