using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpGet("getProjectInfoById")]
        public Project GetProjectInfoById(
            [FromServices] IGetProjectInfoByIdCommand command,
            [FromQuery] Guid projectId)
        {
            return command.Execute(projectId);
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

        [HttpGet("getuserprojects")]
        public IEnumerable<Project> GetUserProjects(
            [FromServices] GetUserProjectsCommand command,
            [FromQuery] Guid userId)
        {
            return command.Execute(userId);
        }
    }
}