using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        [HttpDelete("disableWorkersInProject")]
        public void DisableWorkersInProject(
                    [FromServices] IDisableWorkersInProjectCommand command,
                    [FromQuery] ProjectRequest request)
        {
            command.Execute(request);
        }
    }
}
