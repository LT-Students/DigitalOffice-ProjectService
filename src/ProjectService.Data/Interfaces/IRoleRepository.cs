﻿using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of ProjectService.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Return the role with the specified id from database. 
        /// </summary>
        /// <param name="roleId">Specified id of role.</param>
        /// <returns>Role with specified id.</returns>
        DbRole GetRole(Guid roleId);

        /// <summary>
        /// Return the specified number of roles from database. 
        /// </summary>
        /// <param name="skip">First number of roles to skip.</param>
        /// <param name="take">Number of roles to take.</param>
        /// <returns>Enumerable roles.</returns>
        IEnumerable<DbRole> GetRoles(int skip, int take);
    }
}
