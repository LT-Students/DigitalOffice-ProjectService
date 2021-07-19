using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new project.
    /// </summary>
    [AutoInject]
    public interface ICreateProjectCommand
    {
        /// <summary>
        /// Adds a new project. Returns base info of project .
        /// </summary>
        /// <param name="request">Project data.</param>
        /// <returns>Project info</returns>
        OperationResultResponse<ProjectInfo> Execute(ProjectRequest request);
    }
}
