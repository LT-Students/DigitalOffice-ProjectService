using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting role models.
    /// </summary>
    public interface IGetRolesCommand
    {
        /// <summary>
        /// Returns role models.
        /// </summary>
        /// <returns>Role models.</returns>
        IEnumerable<RolesResponse> Execute();
    }
}
