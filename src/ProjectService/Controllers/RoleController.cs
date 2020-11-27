using System;
using Microsoft.AspNetCore.Mvc;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpDelete("disableUserRoleInProject")]
        public bool DisableUserRoleInProject(
            [FromServices] IDisableRoleCommand command,
            [FromQuery] Guid roleId)
        {
            return command.Execute(roleId);
        }
    }
}
