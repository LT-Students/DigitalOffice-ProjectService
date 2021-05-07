using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskRepository
    {
        DbTask Get(Guid taskId);

        bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch);

        IEnumerable<DbTask> Find(FindTasksFilter filter, IEnumerable<Guid> projectIds, int skipCount, int takeCount, out int totalCount);
    }
}
