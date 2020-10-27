using System;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Deleting role 
        /// </summary>
        /// <param name="roleId">Id of role to be deleted.</param>
        /// <returns>Success of delete.</returns>
        bool DeleteRole(Guid roleId); 
    }
}
