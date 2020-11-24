using System;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpGet("getUserRoleInProject")]
        public RoleExpandedResponse GetUserRoleInProject(
            [FromServices] IGetRoleCommand command,
            [FromQuery] Guid roleId)
        {
            return command.Execute(roleId);
        }

        [HttpGet("getRolesInProject")]
        public RolesResponse GetRolesInProject(
            [FromServices] IGetRolesCommand command,
            [FromQuery] int skip,
            [FromQuery] int take)
        {
            return command.Execute(skip, take);
        }
    }
}