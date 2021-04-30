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
        [HttpPost("create")]
        public OperationResultResponse<Guid> CreateNewTask(
           [FromServices] ICreateTaskCommand command,
           [FromBody] CreateTaskRequest request)
        {
            return command.Execute(request);
        }
    }
}
