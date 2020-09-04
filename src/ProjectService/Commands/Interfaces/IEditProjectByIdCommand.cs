using LT.DigitalOffice.ProjectService.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for editing an existing project.
    /// </summary>
    public interface IEditProjectByIdCommand
    {
        /// <summary>
        /// Calls methods to edit the existing project. Returns the Id of the edited project.
        /// </summary>
        /// <param name="projectId">Id of the project to edit.</param>
        /// <param name="request">New data to update the project with.</param>
        /// <returns></returns>
        Guid Execute(Guid projectId, EditProjectRequest request);
    }
}
