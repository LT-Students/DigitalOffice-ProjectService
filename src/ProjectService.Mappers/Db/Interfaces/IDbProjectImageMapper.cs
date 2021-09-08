﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
    [AutoInject]
    public interface IDbProjectImageMapper
    {
        DbProjectImage Map(CreateImageRequest request, Guid imageId);
    }
}