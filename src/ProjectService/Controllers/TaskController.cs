using System;
using System.Threading.Tasks;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class TaskController : ControllerBase
  {
    [HttpGet("find")]
    public async Task<FindResponse<TaskInfo>> Find(
      [FromServices] IFindTasksCommand command,
      [FromQuery] FindTasksFilter filter,
      [FromQuery] int skipCount,
      [FromQuery] int takeCount)
    {
      return await command.Execute(filter, skipCount, takeCount);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<TaskResponse>> Get(
      [FromQuery] Guid taskId,
      [FromServices] IGetTaskCommand command)
    {
      return await command.Execute(taskId);
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
