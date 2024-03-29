﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
    public interface IDbImageMapper
    {
        DbProjectImage Map(CreateImagesRequest request, Guid imageId);
    }
}
