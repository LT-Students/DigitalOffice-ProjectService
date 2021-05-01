using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDataProvider _provider;

        private IQueryable<DbTask> CreateFindPredicates(
            FindTasksFilter filter,
            IQueryable<DbTask> dbTasks,
            IEnumerable<Guid> projectIds)
        {
            if (filter.Number.HasValue)
            {
                dbTasks = dbTasks.Where(x => x.Number.Equals(filter.Number));
            }

            if (filter.ProjectId.HasValue)
            {
                dbTasks = dbTasks.Where(x => x.ProjectId.Equals(filter.ProjectId));
            }

            if (filter.Assign.HasValue)
            {
                dbTasks = dbTasks.Where(x => x.AssignedTo.Equals(filter.Assign));
            }

            if(projectIds.Any())
            {
                dbTasks = dbTasks.Where(x => projectIds.Contains(x.ProjectId));
            }

            return dbTasks;
        }

        public TaskRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<DbTask> Find(FindTasksFilter filter, IEnumerable<Guid> projectIds, int skipCount, int takeCount, out int totalCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbTasks = _provider.Tasks
                .AsSingleQuery()
                .AsQueryable();

            var tasks = CreateFindPredicates(filter, dbTasks, projectIds).ToList();
            totalCount = tasks.Count;

            return tasks.Skip(skipCount * takeCount).Take(takeCount).ToList();
        }
    }
}
