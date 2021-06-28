using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    public class TaskPropertiesController
    {
        [HttpPost("create")]
        public OperationResultResponse<IEnumerable<Guid>> Create(
            [FromServices] ICreateTaskPropertyCommand command,
            [FromBody] CreateTaskPropertyRequest request)
        {
            return command.Execute(request);
        }

        [HttpGet("find")]
        public FindResponse<TaskPropertyInfo> Find(
            [FromServices] IFindTaskPropertyCommand command,
            [FromQuery] FindTaskPropertiesFilter filter,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount)
        {
            return command.Execute(filter, skipCount, takeCount);
        }
    }
}
