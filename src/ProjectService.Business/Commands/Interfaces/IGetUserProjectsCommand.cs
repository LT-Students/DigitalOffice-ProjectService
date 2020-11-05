using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting list of projects by user id.
    /// </summary>
    public interface IGetUserProjectsCommand
    {
        /// <summary>
        /// Returns list with projects of user id.
        /// </summary>
        /// <param name="userId">Specified id of user.</param>
        /// <returns>List of projects with specified user id.</returns>
        IEnumerable<ProjectResponse> Execute(Guid userId, bool showNotActive);
    }
}
