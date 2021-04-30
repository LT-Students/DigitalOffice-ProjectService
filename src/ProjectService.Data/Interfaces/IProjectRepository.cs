﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    [AutoInject]
    public interface IProjectRepository
    {
        /// <summary>
        /// Returns the project with the specified id from database.
        /// </summary>
        /// <param name="projectId">Specified id of project.</param>
        /// <returns>Project with specified id.</returns>
        DbProject GetProject(Guid projectId);

        /// <summary>
        /// Adds new project to the database. Returns the id of the added project.
        /// </summary>
        /// <param name="item">Project to add.</param>
        void CreateNewProject(DbProject item);

        /// <summary>
        /// Edits the existing project in the database.
        /// </summary>
        /// <param name="dbProject">New data of the project.</param>
        /// <returns>UserId of the edited project.</returns>
        Guid EditProjectById(DbProject dbProject);

        /// <summary>
        /// Disable active workers, which were previously assigned to the project.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="userIds">User ids.</param>
        void DisableWorkersInProject(Guid projectId, IEnumerable<Guid> userIds);

        /// <summary>
        /// Returns all users from project with specified id.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="showNotActiveUsers">Do you want to show inactive users?</param>
        /// <returns>All users from project..</returns>
        IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActiveUsers);
        List<DbProject> FindProjects(FindDbProjectsFilter dbFilter, int skipCount, int takeCount, out int totalCount);

        bool AreExist(params Guid[] ids);
    }
}