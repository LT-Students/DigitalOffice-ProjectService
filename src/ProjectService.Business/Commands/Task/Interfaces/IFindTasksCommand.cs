﻿using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
  [AutoInject]
  public interface IFindTasksCommand
  {
    Task<FindResultResponse<TaskInfo>> Execute(FindTasksFilter filter);
  }
}
