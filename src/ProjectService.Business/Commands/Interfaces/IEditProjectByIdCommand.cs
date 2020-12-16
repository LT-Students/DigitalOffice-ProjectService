using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
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
        /// <param name="request">Data to update the project.</param>
        /// <returns></returns>
        Guid Execute(EditProjectRequest request);
    }
}
