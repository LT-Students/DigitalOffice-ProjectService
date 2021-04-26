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

        public bool Edit(Guid id, JsonPatchDocument<DbTask> taskPatch)
        {
            var dbTask = _provider.Tasks.FirstOrDefault(x => x.Id == id) ??
                         throw new NotFoundException($"Task id '{id}' was not found.");

            taskPatch.ApplyTo(dbTask);
            _provider.Save();

            return true;
        }
    }
}