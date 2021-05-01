
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskRepository
    {
        IEnumerable<DbTask> Find(FindTasksFilter filter, IEnumerable<Guid> projectIds, int skipCount, int takeCount, out int totalCount);
    }
}
