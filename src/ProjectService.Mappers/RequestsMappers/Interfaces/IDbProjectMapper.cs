using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
<<<<<<< HEAD:src/ProjectService.Mappers/RequestsMappers/Interfaces/IEditProjectMapper.cs
using Microsoft.AspNetCore.JsonPatch;
=======
using System;
>>>>>>> develop:src/ProjectService.Mappers/RequestsMappers/Interfaces/IDbProjectMapper.cs

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
<<<<<<< HEAD:src/ProjectService.Mappers/RequestsMappers/Interfaces/IEditProjectMapper.cs
    public interface IEditProjectMapper
    {
        JsonPatchDocument<DbProject> Map(JsonPatchDocument<EditProjectRequest> request);
=======
    public interface IDbProjectMapper
    {
        public DbProject Map(ProjectRequest request, Guid authorId);
>>>>>>> develop:src/ProjectService.Mappers/RequestsMappers/Interfaces/IDbProjectMapper.cs
    }
}
