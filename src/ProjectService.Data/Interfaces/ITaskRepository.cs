using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.JsonPatch;
﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    [AutoInject]
    public interface ITaskRepository
    {
        DbTask Get(Guid taskId);

        bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch);

        IEnumerable<DbTask> Find(FindTasksFilter filter, IEnumerable<Guid> projectIds, int skipCount, int takeCount, out int totalCount);
    }
}