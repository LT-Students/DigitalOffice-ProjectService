using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new role.
    /// </summary>
    public interface ICreateRoleCommand
    {
        /// <summary>
        /// Adds a new role. Returns id of the added role.
        /// </summary>
        /// <param name="request">Role data.</param>
        /// <returns>Id of the added role.</returns>
        Guid Execute(CreateRoleRequest request);
    }
}
