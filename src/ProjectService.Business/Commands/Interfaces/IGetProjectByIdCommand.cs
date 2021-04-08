using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting project model by id.
    /// </summary>
    [AutoInject]
    public interface IGetProjectByIdCommand
    {
        /// <summary>
        /// Returns the project model with the specified id.
        /// </summary>
        /// <param name="projectId">Specified id of project.</param>
        /// <returns>Project expanded information.</returns>
        Task<ProjectExpandedResponse> Execute(Guid projectId, bool showNotActiveUsers);
    }
}