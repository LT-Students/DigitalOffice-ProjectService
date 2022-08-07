using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Department.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Department;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class DepartmentController : ControllerBase
  {
    [HttpPut("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditProjectDepartmentCommand command,
      [FromBody] EditProjectDepartmentRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
