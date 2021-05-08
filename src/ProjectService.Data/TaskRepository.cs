using System;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

            if (filter.AssignedTo.HasValue)
            {
                dbTasks = dbTasks.Where(x => x.AssignedTo.Equals(filter.AssignedTo));
            }

            if (projectIds.Any())
            {
                dbTasks = dbTasks.Where(x => projectIds.Contains(x.ProjectId));
            }

            return dbTasks;
        }

        public TaskRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public Guid CreateTask(DbTask newTask)
        {
            _provider.Tasks.Add(newTask);
            _provider.Save();

            return newTask.Id;
        }

        public bool IsExist(Guid id)
        {
            return _provider.Tasks.FirstOrDefault(x => x.ParentId == id) != null;
        }

        public bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch)
        {
            taskPatch.ApplyTo(task);
            _provider.Save();

            return true;
        }
        public DbTask Get(Guid taskId)
        {
            return _provider.Tasks.FirstOrDefault(x => x.Id == taskId) ??
                throw new NotFoundException($"Task id '{taskId}' was not found.");
        }

        public IEnumerable<DbTask> Find(
            FindTasksFilter filter,
            IEnumerable<Guid> projectIds,
            int skipCount,
            int takeCount,
            out int totalCount)
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