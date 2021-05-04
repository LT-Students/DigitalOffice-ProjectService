using System;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;

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
            return _provider.Tasks.FirstOrDefault(x => x.Id == taskId) ??
                   throw new NotFoundException($"Task id '{taskId}' was not found.");
        }
    }
}
