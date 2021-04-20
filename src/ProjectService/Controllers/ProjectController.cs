using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpGet("find")]
        public ProjectsResponse Find(
            [FromServices] IFindProjectsCommand command,
            [FromQuery] FindProjectsFilter filter,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount)
        {
            return command.Execute(filter, skipCount, takeCount);
        }

        [HttpGet("getProjectById")]
        public ProjectExpandedResponse GetProjectById(
            [FromServices] IGetProjectByIdCommand command,
            [FromQuery] Guid projectId,
            [FromQuery] bool showNotActiveUsers = false)
        {
            return command.Execute(projectId, showNotActiveUsers).Result;
        }

        [HttpPost("createNewProject")]
        public Guid CreateNewProject(
            [FromServices] ICreateNewProjectCommand command,
            [FromBody] ProjectExpandedRequest request)
        {
            return command.Execute(request);
        }

        [HttpPost("editProjectById")]
        public Guid EditProjectById(
            [FromServices] IEditProjectByIdCommand command,
            [FromBody] EditProjectRequest request)
        {
            return command.Execute(request);
        }
    }
}