using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpPost("createRole")]
        public Guid CreateRole(
            [FromServices] ICreateRoleCommand command,
            [FromBody] CreateRoleRequest request) => command.Execute(request);
        
        [HttpDelete("disableUserRoleInProject")]
        public bool DisableUserRoleInProject(
            [FromServices] IDisableRoleCommand command,
            [FromQuery] Guid roleId)
        {
            return command.Execute(roleId);
        }
    }
}
