using LT.DigitalOffice.ProjectService.Models.Db;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    public interface IRoleRepository
    {      
        /// <summary>
        /// Adds new role to the database. Returns the id of the added role.
        /// </summary>
        /// <param name="role">Role to add.</param>
        /// <returns>Id of the added role.</returns>
        Guid CreateRole(DbRole role);

        /// <summary>
        /// Returns roleId from database.
        /// </summary>
        /// <param name="roleId"> Role to find. </param>
        /// <returns> Role with specified id. </returns>
        DbRole GetRole(Guid roleId);

        /// <summary>
        /// Deleting role. 
        /// </summary>
        /// <param name="roleId">Id of role to be deleted.</param>
        /// <returns>Success of delete.</returns>
        bool DisableRole(Guid roleId); 
    }
}