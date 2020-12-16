using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
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

        [HttpPost("editProjectUserById")]
        public void EditProjectUserById(
                    [FromServices] IEditProjectUserByIdCommand command,
                    [FromBody] EditProjectUserRequest request)
        {
            command.Execute(request);
        }
    }
}
