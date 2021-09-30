using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
    [ApiController]
    public class TaskPropertyController : ControllerBase
    {
        private readonly IHttpContextAccessor _context;

        public TaskPropertyController(IHttpContextAccessor context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public OperationResultResponse<IEnumerable<Guid>> Create(
            [FromServices] ICreateTaskPropertyCommand command,
            [FromBody] CreateTaskPropertyRequest request)
        {
            _context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

            return command.Execute(request);
        }

        [HttpGet("find")]
        public FindResultResponse<TaskPropertyInfo> Find(
            [FromServices] IFindTaskPropertyCommand command,
            [FromQuery] FindTaskPropertiesFilter filter,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount)
        {
            return command.Execute(filter, skipCount, takeCount);
        }
    }
}
