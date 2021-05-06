using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDataProvider _provider;

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
}

        public bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch)
        {
            taskPatch.ApplyTo(task);
            _provider.Save();
        }

            return true;
        public DbTask Get(Guid taskId)

        {
            return _provider.Tasks.FirstOrDefault(x => x.Id == taskId) ??
                   throw new NotFoundException($"Task id '{taskId}' was not found.");
        }
    }
using LT.DigitalOffice.Kernel.Exceptions.Models;
using Microsoft.AspNetCore.JsonPatch;