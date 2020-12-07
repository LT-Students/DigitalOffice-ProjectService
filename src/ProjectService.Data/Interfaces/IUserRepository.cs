using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Add users to project.
        /// </summary>
        /// <param name="dbProjectUser">Id of role to be deleted.</param>
        /// <returns>Success of delete.</returns>
        void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUser);
    }
}
