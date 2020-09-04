using LT.DigitalOffice.ProjectService.Models;

namespace LT.DigitalOffice.ProjectService.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// </summary>
    public interface IDisableWorkersInProjectCommand
    {
        /// <summary>
        /// Call repository for disabling workers from project.
        /// </summary>
        /// <param name="request"></param>
        void Execute(WorkersIdsInProjectRequest request);
    }
}
