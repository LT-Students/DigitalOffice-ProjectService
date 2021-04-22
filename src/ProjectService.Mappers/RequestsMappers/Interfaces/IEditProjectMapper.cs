using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    public interface IEditProjectMapper : IMapper<JsonPatchDocument<EditProjectRequest>, JsonPatchDocument<DbProject>>
    {
    }
}
