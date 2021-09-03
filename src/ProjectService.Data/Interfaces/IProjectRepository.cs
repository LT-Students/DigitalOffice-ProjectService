using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Project;
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
        DbProject Get(GetProjectFilter filter);

        IEnumerable<DbProject> Get(Guid departmentId);

        List<DbProject> Find(List<Guid> projectIds);

        /// <summary>
        /// Adds new project to the database. Returns the id of the added project.
        /// </summary>
        /// <param name="dbProject">Project to add.</param>
        Guid Create(DbProject dbProject);

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

        List<DbProject> Find(FindProjectsFilter filter, int skipCount, int takeCount, out int totalCount);

        List<DbProject> Search(string text);

        List<DbProject> Get(IGetProjectsRequest request, out int totalCount);

        bool IsExist(Guid id);

        bool IsProjectNameExist(string name);
    }
}