using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDataProvider provider;

        public TaskRepository(IDataProvider provider)
        {
            this.provider = provider;
        }

        public Guid CreateTask(DbTask newTask)
        {
            provider.Tasks.Add(newTask);
            provider.Save();

            return newTask.Id;
        }

        public bool IsExist(Guid id)
        {
            return provider.Tasks.FirstOrDefault(x => x.ParentTaskId == id) != null;
        }
    }
}
