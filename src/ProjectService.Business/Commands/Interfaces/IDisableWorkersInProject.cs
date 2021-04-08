using LT.DigitalOffice.Kernel.Attributes;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// </summary>
    [AutoInject]
    public interface IDisableWorkersInProjectCommand
    {
        /// <summary>
        /// Call repository for disabling workers from project.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="userIds">User ids.</param>
        void Execute(Guid projectId, IEnumerable<Guid> userIds);
    }
}
