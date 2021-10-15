using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    [HttpPost("addUsersToProject")]
    public OperationResultResponse<bool> AddUsersToProject(
      [FromServices] IAddUsersToProjectCommand command,
      [FromBody] AddUsersToProjectRequest request)
    {
      return command.Execute(request);
    }

    [HttpDelete("removeUsersFromProject")]
    public OperationResultResponse<bool> RemoveUsersFromProject(
      [FromServices] IRemoveUsersFromProjectCommand command,
      [FromQuery] Guid projectId,
      [FromBody] List<Guid> userIds)
    {
      return command.Execute(projectId, userIds);
    }
  }
}
