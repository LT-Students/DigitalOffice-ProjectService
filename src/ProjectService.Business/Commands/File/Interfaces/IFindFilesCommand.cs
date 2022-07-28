﻿using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.File;

namespace LT.DigitalOffice.ProjectService.Business.Commands.File.Interfaces
{
  [AutoInject]
  public interface IFindFilesCommand
  {
    Task<FindResultResponse<FileCharacteristicsData>> ExecuteAsync(Guid projectId);
  }
}
