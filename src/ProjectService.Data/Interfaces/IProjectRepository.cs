﻿using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
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
        /// <returns>Id of the added project.</returns>
        Guid CreateNewProject(DbProject item);

        /// <summary>
        /// Edits the existing project in the database.
        /// </summary>
        /// <param name="dbProject">New data of the project.</param>
        /// <returns>Id of the edited project.</returns>
        Guid EditProjectById(DbProject dbProject);

        /// <summary>
        /// Disable active workers, which were previously assigned to the project.
        /// </summary>
        /// <param name="request">Contains workers id and project id.</param>
        void DisableWorkersInProject(WorkersIdsInProjectRequest request);

        IEnumerable<DbProject> GetProjects(bool showNotActive);

        IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActiveUsers);
    }
}