using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto;

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
