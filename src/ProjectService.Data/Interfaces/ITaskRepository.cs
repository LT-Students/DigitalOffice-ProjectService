using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskRepository
    {
        public bool Edit(Guid id, JsonPatchDocument<DbTask> taskPatch);

        public DbTask Get(Guid taskId);
    }
}
