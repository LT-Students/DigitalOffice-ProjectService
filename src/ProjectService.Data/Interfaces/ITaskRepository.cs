using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;
﻿using LT.DigitalOffice.Kernel.Attributes;
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

        /// <summary>
        /// Checking the task Id in database. Returns true if it exist or false if it's not.
        /// </summary>
        /// <param name="Id">task Id.</param>
        /// <returns>true or false after checking the task Id.</returns>
        bool IsExist(Guid id);

        public bool Edit(DbTask task, JsonPatchDocument<DbTask> taskPatch);

        public DbTask Get(Guid taskId);
    }
}