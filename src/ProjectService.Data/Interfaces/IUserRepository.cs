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

        bool AreUserAndProjectExist(Guid usderId, Guid projectId);
    }
}
