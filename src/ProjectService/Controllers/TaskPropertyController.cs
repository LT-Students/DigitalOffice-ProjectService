using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class TaskPropertyController : ControllerBase
  {
    private readonly IHttpContextAccessor _context;

    public TaskPropertyController(IHttpContextAccessor context)
    {
      _context = context;
    }

    [HttpPost("create")]
    public OperationResultResponse<List<Guid>> Create(
        [FromServices] ICreateTaskPropertyCommand command,
        [FromBody] CreateTaskPropertyRequest request)
    {
      return command.Execute(request);
    }

    [HttpGet("find")]
    public FindResultResponse<TaskPropertyInfo> Find(
      [FromServices] IFindTaskPropertyCommand command,
      [FromQuery] FindTaskPropertiesFilter filter)
    {
      return command.Execute(filter);
    }

    [HttpPatch("edit")]
    public OperationResultResponse<bool> Edit(
      [FromQuery] Guid taskPropertyId,
      [FromBody] JsonPatchDocument<TaskProperty> request,
      [FromServices] IEditTaskPropertyCommand command)
    {
      return command.Execute(taskPropertyId, request);
    }
  }
}
