using LT.DigitalOffice.ProjectService.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting project model by id.
    /// </summary>
    public interface IGetProjectInfoByIdCommand
    {
        /// <summary>
        /// Returns the project model with the specified id.
        /// </summary>
        /// <param name="projectId">Specified id of project.</param>
        /// <returns>Project model with specified id.</returns>
        Project Execute(Guid projectId);
    }
}