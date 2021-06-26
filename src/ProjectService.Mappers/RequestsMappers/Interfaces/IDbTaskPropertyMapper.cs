using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    public interface IDbTaskPropertyMapper
    {
        DbTaskProperty Map(TaskProperty request);
    }
}
