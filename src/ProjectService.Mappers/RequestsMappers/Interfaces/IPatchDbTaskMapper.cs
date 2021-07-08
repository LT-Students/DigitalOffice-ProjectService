using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
    public interface IPatchDbTaskMapper
    {
        JsonPatchDocument<DbTask> Map(JsonPatchDocument<EditTaskRequest> request);
    }
}
