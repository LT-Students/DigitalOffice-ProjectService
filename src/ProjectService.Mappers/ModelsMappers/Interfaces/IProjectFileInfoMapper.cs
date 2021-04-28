﻿using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces
{
    [AutoInject]
    public interface IProjectFileInfoMapper
    {
        ProjectFileInfo Map(DbProjectFile dbProjectFile);
    }
}
