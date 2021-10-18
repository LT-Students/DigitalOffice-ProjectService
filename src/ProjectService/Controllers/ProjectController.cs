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
    public async Task<FindResultResponse<ProjectInfo>> FindAsync(
      [FromServices] IFindProjectsCommand command,
      [FromQuery] FindProjectsFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<ProjectResponse>> GetAsync(
      [FromServices] IGetProjectCommand command,
      [FromQuery] GetProjectFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }

    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreateProjectCommand command,
      [FromBody] CreateProjectRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditProjectCommand command,
      [FromQuery] Guid projectId,
      [FromBody] JsonPatchDocument<EditProjectRequest> request)
    {
      return await command.ExecuteAsync(projectId, request);
    }
  }
}
