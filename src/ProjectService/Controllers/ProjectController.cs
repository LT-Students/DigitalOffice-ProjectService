using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpGet("getProjects")]
        public IEnumerable<ProjectInfo> GetProjects(
            [FromServices] IGetProjectsCommand command,
            [FromQuery] bool showNotActive = false)
        {
            return command.Execute(showNotActive);
        }

        [HttpGet("getProjectById")]
        public ProjectExpandedResponse GetProjectById(
            [FromServices] IGetProjectByIdCommand command,
            [FromQuery] Guid projectId,
            [FromQuery] bool showNotActiveUsers = false)
        {
            return command.Execute(projectId, showNotActiveUsers).Result;
        }

        [HttpPost("create")]
        public OperationResultResponse<ProjectInfo> Create(
            [FromServices] ICreateProjectCommand command,
            [FromBody] ProjectRequest request)
        {
            return command.Execute(request);
        }

        [HttpPatch("edit")]
        public bool Edit(
            [FromServices] IEditProjectCommand command,
            [FromQuery] Guid projectId,
            [FromBody] JsonPatchDocument<EditProjectRequest> request)
        {
            return command.Execute(projectId, request);
        }
    }
}