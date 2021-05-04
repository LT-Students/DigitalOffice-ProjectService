using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using Microsoft.AspNetCore.JsonPatch;

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

        [HttpPost("create")]
        public OperationResultResponse<ProjectInfo> Create(
            [FromServices] ICreateProjectCommand command,
            [FromBody] ProjectRequest request)
        {
            return command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromServices] IEditProjectCommand command,
            [FromQuery] Guid projectId,
            [FromBody] JsonPatchDocument<EditProjectRequest> request)
        {
            return command.Execute(projectId, request);
        }
    }
}