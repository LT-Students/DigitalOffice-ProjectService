using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting role model by id.
    /// </summary>
    public interface IGetRoleCommand
    {
        /// <summary>
        /// Returns the role model with the specified id.
        /// </summary>
        /// <param name="roleId">Specified id of role.</param>
        /// <returns>Role information.</returns>
        RoleExpandedResponse Execute(Guid roleId);
    }
}
