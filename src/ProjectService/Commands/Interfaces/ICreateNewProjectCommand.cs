using LT.DigitalOffice.ProjectService.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new project.
    /// </summary>
    public interface ICreateNewProjectCommand
    {
        /// <summary>
        /// Adds a new project. Returns id of the added project.
        /// </summary>
        /// <param name="request">Project data.</param>
        /// <returns>Id of the added project.</returns>
        Guid Execute(NewProjectRequest request);
    }
}
