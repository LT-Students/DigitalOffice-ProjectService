using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    [AutoInject]
    public interface ITaskRepository
    {
        /// <summary>
        /// Adds new task to the database. Returns the id of the added task.
        /// </summary>
        /// <param name="item">Task to add.</param>
        /// <returns>Id of the added task.</returns>
        Guid CreateTask(DbTask item);

        bool AreExist(Guid id);
    }
}
