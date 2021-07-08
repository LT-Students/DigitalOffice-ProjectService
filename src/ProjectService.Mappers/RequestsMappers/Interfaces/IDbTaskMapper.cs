using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
    public interface IDbTaskMapper
    {
        DbTask Map(CreateTaskRequest taskRequest);
    }
}
