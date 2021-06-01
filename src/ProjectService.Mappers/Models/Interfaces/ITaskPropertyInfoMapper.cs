﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
 
namespace LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces
{
    [AutoInject()]
    public interface ITaskPropertyInfoMapper
    {
        TaskPropertyInfo Map(DbTaskProperty dbTaskProperty);
    }
}