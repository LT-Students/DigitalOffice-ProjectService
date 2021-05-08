using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDataProvider _provider;
        
        public TaskRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch)
        {
            taskPatch.ApplyTo(task);
            _provider.Save();

            return true;
        }

        public DbTask Get(Guid taskId)
        {
            DbTask dbTask = _provider.Tasks
                                .Include(t => t.Project)
                                .Include(t => t.Author)
                                .Include(t => t.AssignedUser)
                                .Include(t => t.Status)
                                .Include(t => t.Priority)
                                .Include(t => t.Type)
                                .Include(t => t.Subtasks)
                                .FirstOrDefault(x => x.Id == taskId) ??
                throw new NotFoundException($"Task id '{taskId}' was not found.");

            return dbTask;
        }
    }
}
