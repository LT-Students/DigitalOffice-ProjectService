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
        /// Deleting role.
        /// </summary>
        /// <param name="roleId">Id of role to be deleted.</param>
        /// <returns>Success of delete.</returns>
        bool DisableRole(Guid roleId);
    }
}
