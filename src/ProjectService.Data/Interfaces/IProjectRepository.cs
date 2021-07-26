using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.JsonPatch;
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
        /// <param name="filter">Filter info.</param>
        /// <returns>Project with specified id.</returns>
        public DbProject GetProject(GetProjectFilter filter);

        public IEnumerable<DbProject> Get(Guid departmentId);

        public List<DbProject> Find(List<Guid> projectIds);

        /// <summary>
        /// Adds new project to the database. Returns the id of the added project.
        /// </summary>
        /// <param name="item">Project to add.</param>
        void CreateNewProject(DbProject item);

        /// <summary>
        /// Edits the existing project in the database.
        /// </summary>
        /// <param name="projectId">Edit project Id.</param>
        /// <param name="request">New data of the project.</param>
        /// <returns>True if project updated.</returns>
        bool Edit(DbProject dbProject, JsonPatchDocument<DbProject> request);

        /// <summary>
        /// Disable active workers, which were previously assigned to the project.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="userIds">User ids.</param>
        void DisableWorkersInProject(Guid projectId, IEnumerable<Guid> userIds);

        List<DbProject> FindProjects(FindProjectsFilter filter, int skipCount, int takeCount, out int totalCount);

        List<DbProject> Search(string text);

        bool IsExist(Guid id);

        bool IsProjectNameExist(string name);
    }
}