using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    public interface ITaskResponseMapper
    {
        public TaskResponse Map(DbTask dbTask);
    }
}