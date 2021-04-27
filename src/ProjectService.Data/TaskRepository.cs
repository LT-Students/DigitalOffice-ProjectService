using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskRepository
    {
        private readonly IDataProvider _provider;

        public TaskRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public Guid Create(DbTask newTask)
        {
            _provider.Tasks.Add(newTask);
            _provider.Save();

            return newTask.Id;
        }

        public IEnumerable<DbTask> Find()
        {

            return
        }
    }
}
