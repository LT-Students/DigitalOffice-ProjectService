using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Adds new role to the database. Returns the id of the added role.
        /// </summary>
        /// <param name="item">Role to add.</param>
        /// <returns>Id of the added role.</returns>
        Guid CreateRole(DbRole item);
    }
}
