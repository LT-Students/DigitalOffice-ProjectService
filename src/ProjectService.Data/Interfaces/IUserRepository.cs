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
        /// Add users to project.
        /// </summary>
        /// <param name="dbProjectUsers">List project users to add.</param>
        /// <param name="projectId">Project id from request.</param>
        void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUsers, Guid projectId);

        IEnumerable<DbProjectUser> Find(Guid userId);

        bool AreUserProjectExist(Guid userId, Guid projectId);

        /// <summary>
        /// Check that users are exist
        /// </summary>
        /// <param name="ids">Ids to check that all of them exists</param>
        bool AreExist(params Guid[] ids);
    }
}
