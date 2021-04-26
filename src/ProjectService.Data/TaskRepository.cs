using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDataProvider provider;

        public TaskRepository(IDataProvider provider)
        {
            this.provider = provider;
        }

        public Guid CreateNewTask(DbTask newTask)
        {
            provider.Task.Add(newTask);
            provider.Save();

            return newTask.Id;
        }

        /*public bool IsExist(params Guid[] ids)
        {
            provider.Task.
        }*/
    }
}
