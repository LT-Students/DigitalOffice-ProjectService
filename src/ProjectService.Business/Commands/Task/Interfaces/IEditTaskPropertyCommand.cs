﻿using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces
{
  [AutoInject]
  public interface IEditTaskPropertyCommand
  {
    OperationResultResponse<bool> Execute(Guid taskPropertyId, JsonPatchDocument<EditTaskPropertyRequest> patch);
  }
}
