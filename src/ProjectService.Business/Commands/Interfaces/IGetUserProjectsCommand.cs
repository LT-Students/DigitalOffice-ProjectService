using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using System;
using System.Collections.Generic;

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
        List<DbProject> Execute(Guid userId);
    }
}
