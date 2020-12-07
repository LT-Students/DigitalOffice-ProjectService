using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        [HttpPost("addUsersToProject")]
        public void AddUsersToProject(
            [FromServices] IAddUsersToProjectCommand command,
            [FromQuery] AddUsersToProjectRequest request)
        {
            command.Execute(request);
        }

        [HttpDelete("disableWorkersInProject")]
        public void DisableWorkersInProject(
                    [FromServices] IDisableWorkersInProjectCommand command,
                    [FromQuery] ProjectExpandedRequest request)
        {
            command.Execute(request);
        }
    }
}
