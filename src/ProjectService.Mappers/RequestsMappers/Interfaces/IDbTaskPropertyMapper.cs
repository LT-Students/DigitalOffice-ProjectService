using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    public interface IDbTaskPropertyMapper
    {
        DbTaskProperty Map(TaskProperty request, Guid userId);
    }
}
