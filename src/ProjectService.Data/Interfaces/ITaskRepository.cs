using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskRepository
    {
        public bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch);

        public DbTask Get(Guid taskId);
    }
}
