using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [HttpPost("Create")]
        public OperationResultResponse<Guid> Create(
           [FromServices] ICreateTaskCommand command,
           [FromBody] CreateTaskRequest request)
        {
            return command.Execute(request);
        }
    }
}

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromQuery] Guid id,
            [FromBody] JsonPatchDocument<EditTaskRequest> request,
            IEditTaskCommand command)
        {
            return command.Execute(id, request);
        }
    }
}

using Microsoft.AspNetCore.JsonPatch;