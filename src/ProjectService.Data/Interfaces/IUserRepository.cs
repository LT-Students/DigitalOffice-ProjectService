using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    [AutoInject]
    public interface IUserRepository
    {
        /// <summary>
        /// Returns all users from project with specified id.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="showNotActiveUsers">Include not active users.</param>
        IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActiveUsers);

        /// <summary>
        /// Add users to project.
        /// </summary>
        /// <param name="dbProjectUsers">List project users to add.</param>
        /// <param name="projectId">Project id from request.</param>
        void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUsers, Guid projectId);

        /// <summary>
        /// Get user projects.
        /// </summary>
        /// <param name="userId">User Id from request</param>
        IEnumerable<DbProjectUser> Find(Guid userId);
        /// <param name="filter">Properties to filter query.</param>
        IEnumerable<DbProjectUser> Find(FindDbProjectsUserFilter filter);

        bool AreUserProjectExist(Guid userId, Guid projectId);

        /// <summary>
        /// Check that users are exist.
        /// </summary>
        /// <param name="ids">Ids to check that all of them exists.</param>
        bool AreExist(params Guid[] ids);

        List<DbProjectUser> Find(List<Guid> userIds);

        void Remove(Guid userId);
    }
}
