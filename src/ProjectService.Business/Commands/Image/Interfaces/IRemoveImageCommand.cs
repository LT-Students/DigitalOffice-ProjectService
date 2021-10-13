﻿using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface IRemoveImageCommand
  {
    Task<OperationResultResponse<bool>> Execute(RemoveImageRequest request);
  }
}
