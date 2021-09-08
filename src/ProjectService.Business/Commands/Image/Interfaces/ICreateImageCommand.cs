﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces
{
    [AutoInject]
    public interface ICreateImageCommand
    {
        OperationResultResponse<List<Guid>> Execute(CreateImageRequest request);
    }
}