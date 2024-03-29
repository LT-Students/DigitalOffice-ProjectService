﻿using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
  [AutoInject]
  public interface ICreateProjectCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateProjectRequest request);
  }
}
