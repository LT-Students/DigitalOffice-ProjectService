using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;

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
        /// <param name="roleId"> Role to find </param>
        /// <returns> Role </returns>
        DbRole GetRole(Guid roleId);
    }
}