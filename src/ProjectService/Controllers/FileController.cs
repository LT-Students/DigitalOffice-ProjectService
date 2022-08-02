﻿using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class FileController : ControllerBase
  {
    [HttpGet("find")]
    public async Task<FindResultResponse<FileCharacteristicsData>> FindAsync(
      [FromServices] IFindFilesCommand command,
      [FromQuery] FindProjectFilesFilter findFilter)
    {
      return await command.ExecuteAsync(findFilter);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveFilesCommand command,
      [FromBody] RemoveFilesRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
