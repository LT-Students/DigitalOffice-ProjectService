using System;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ProjectController : ControllerBase
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectController(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("find")]
    public async Task<FindResponse<ProjectInfo>> Find(
      [FromServices] IFindProjectsCommand command,
      [FromQuery] FindProjectsFilter filter,
      [FromQuery] int skipCount,
      [FromQuery] int takeCount)
    {
      return await command.Execute(filter, skipCount, takeCount);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<ProjectResponse>> Get(
      [FromServices] IGetProjectCommand command,
      [FromQuery] GetProjectFilter filter)
    {
      return await command.Execute(filter);
    }

    [HttpPost("create")]
    public OperationResultResponse<Guid> Create(
      [FromServices] ICreateProjectCommand command,
      [FromBody] CreateProjectRequest request)
    {
      var result = command.Execute(request);

      if (result.Status == OperationResultStatusType.Failed)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

        return result;
      }

      if (result.Status != OperationResultStatusType.Failed)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      }

      return result;
    }

    [HttpPatch("edit")]
    public OperationResultResponse<bool> Edit(
      [FromServices] IEditProjectCommand command,
      [FromQuery] Guid projectId,
      [FromBody] JsonPatchDocument<EditProjectRequest> request)
    {
      var result = command.Execute(projectId, request);

      if (result.Status == OperationResultStatusType.Failed)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
      }

      return result;
    }
  }
}
