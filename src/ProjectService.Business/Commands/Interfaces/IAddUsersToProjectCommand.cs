using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new users to project.
    /// </summary>
    public interface IAddUsersToProjectCommand
    {
        /// <summary>
        /// Added new users id to specific project.
        /// </summary>
        /// <param name="request">List of users for a specific project.</param>
        void Execute(AddUsersToProjectRequest request);
    }
}