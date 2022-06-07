using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [HttpPost("create")]
    public async Task<OperationResultResponse<bool>> CreateAsync(
      [FromServices] ICreateProjectUsersCommand command,
      [FromBody] CreateProjectUsersRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPut("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditProjectUsersRoleCommand command,
      [FromQuery] Guid projectId,
      [FromBody] EditProjectUsersRoleRequest request)
    {
      return await command.ExecuteAsync(projectId, request);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveProjectUsersCommand command,
      [FromQuery] Guid projectId,
      [FromBody] List<Guid> usersIds)
    {
      return await command.ExecuteAsync(projectId, usersIds);
    }
  }
}
