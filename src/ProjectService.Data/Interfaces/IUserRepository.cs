using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
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
        /// <param name="showNotActiveUsers">Do you want to show inactive users?</param>
        /// <returns>All users from project..</returns>
        IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActiveUsers);

        /// <summary>
        /// Add users to project.
        /// </summary>
        /// <param name="dbProjectUsers">List project users to add.</param>
        /// <param name="projectId">Project id from request.</param>
        void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUsers, Guid projectId);
    }
}
