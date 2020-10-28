using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpGet("getProjects")]
        public IEnumerable<ProjectResponse> GetProjects(
            [FromServices] IGetProjectsCommand command,
            [FromQuery] bool showNotActive = false)
        {
            return command.Execute(showNotActive);
        }

        [HttpGet("getProject")]
        public ProjectExpandedResponse GetProject(
            [FromServices] IGetProjectCommand command,
            [FromQuery] Guid projectId,
            [FromQuery] bool showNotActiveUsers = false)
        {
            return command.Execute(projectId, showNotActiveUsers).Result;
        }

        [HttpPost("createNewProject")]
        public Guid CreateNewProject(
            [FromServices] ICreateNewProjectCommand command,
            [FromBody] NewProjectRequest request) => command.Execute(request);

        [HttpPut("editProjectById")]
        public Guid EditProjectById(
            [FromServices] IEditProjectByIdCommand command,
            [FromQuery] Guid projectId,
            [FromBody] EditProjectRequest request)
        {
            return command.Execute(projectId, request);
        }

        [HttpDelete("disableWorkersInProject")]
        public void DisableWorkersInProject(
            [FromServices] IDisableWorkersInProjectCommand command,
            [FromQuery] WorkersIdsInProjectRequest request)
        {
            command.Execute(request);
        }

        [HttpGet("getUserProjects")]
        public IEnumerable<Project> GetUserProjects(
            [FromServices] GetUserProjectsCommand command,
            [FromQuery] Guid userId)
        {
            return command.Execute(userId);
        }
    }
}