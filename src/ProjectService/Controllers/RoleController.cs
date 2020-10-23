﻿using Microsoft.AspNetCore.Mvc;
using System;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;

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
    }
}
