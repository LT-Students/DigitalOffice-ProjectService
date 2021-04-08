using LT.DigitalOffice.Kernel.Attributes;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for disabling user role in project.
    /// </summary>
    [AutoInject]
    public interface IDisableRoleCommand
    {
        /// <summary>
        /// Call repository for disabling role in project.
        /// </summary>
        /// <param name="roleId">Id of role to be deleted.</param>
        /// <returns>Returns success of delete.</returns>
        bool Execute(Guid roleId);
    }
}
