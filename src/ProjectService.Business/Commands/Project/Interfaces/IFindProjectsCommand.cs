﻿﻿using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
  [AutoInject]
  public interface IFindProjectsCommand
  {
    Task<FindResultResponse<ProjectInfo>> ExecuteAsync(FindProjectsFilter filter);
  }
}
