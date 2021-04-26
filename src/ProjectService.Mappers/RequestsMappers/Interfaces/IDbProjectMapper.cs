﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
    public interface IDbProjectMapper
    {
        public DbProject Map(ProjectRequest request, Guid authorId);
    }
}