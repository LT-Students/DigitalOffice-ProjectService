using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ProjectController : ControllerBase
  {
    [HttpGet("find")]
    public async Task<FindResultResponse<ProjectInfo>> Find(
      [FromServices] IFindProjectsCommand command,
      [FromQuery] FindProjectsFilter filter)
    {
      return await command.Execute(filter);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<ProjectResponse>> Get(
      [FromServices] IGetProjectCommand command,
      [FromQuery] GetProjectFilter filter)
    {
      return await command.Execute(filter);
    }

    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> Create(
      [FromServices] ICreateProjectCommand command,
      [FromBody] CreateProjectRequest request)
    {
      return await command.Execute(request);
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
