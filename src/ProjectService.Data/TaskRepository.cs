using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class TaskRepository : ITaskRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

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

      if (filter.Status.HasValue)
      {
        dbTasks = dbTasks.Where(x => x.Status.Id.Equals(filter.Status));
      }

      if (projectIds.Any())
      {
        dbTasks = dbTasks.Where(x => projectIds.Contains(x.ProjectId));
      }

      return dbTasks;
    }

    public TaskRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public Guid Create(DbTask newTask)
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
      task.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      task.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();

      return true;
    }

    public DbTask Get(Guid taskId, bool isFullModel)
    {
      if (isFullModel)
      {
        DbTask dbTask = _provider.Tasks
          .Include(t => t.Project)
          .Include(t => t.Status)
          .Include(t => t.Priority)
          .Include(t => t.Type)
          .Include(t => t.Subtasks)
          .Include(t => t.Images)
          .FirstOrDefault(x => x.Id == taskId) ??
            throw new NotFoundException($"Task id '{taskId}' was not found.");

        return dbTask;
      }

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
      if (skipCount < 0)
      {
        throw new BadRequestException("Skip count can't be less than 0.");
      }

      if (takeCount < 1)
      {
        throw new BadRequestException("Take count can't be less than 1.");
      }

      if (filter == null)
      {
        throw new ArgumentNullException(nameof(filter));
      }

      IQueryable<DbTask> dbTasks = _provider.Tasks
        .Include(t => t.Priority)
        .Include(t => t.Type)
        .Include(t => t.Status)
        .Include(t => t.Project)
        .AsSingleQuery()
        .AsQueryable();

      IQueryable<DbTask> tasks = CreateFindPredicates(filter, dbTasks, projectIds);
      totalCount = tasks.Count();

      return tasks.Skip(skipCount).Take(takeCount).ToList();
    }
  }
}
