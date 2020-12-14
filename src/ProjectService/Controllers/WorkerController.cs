using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpDelete("removeUsersFromProject")]
        public void RemoveUsersFromProject(
                    [FromServices] IDisableWorkersInProjectCommand command,
                    [FromQuery] Guid projectId,
                    [FromQuery] Guid[] userIds)
        {
            command.Execute(projectId, userIds);
        }
    }
}
