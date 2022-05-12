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
      [FromBody] ProjectUsersRequest request)
    {
      return await command.ExecuteAsync(request);
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
