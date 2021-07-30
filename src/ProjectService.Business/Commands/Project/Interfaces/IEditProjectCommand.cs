using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for editing an existing project.
    /// </summary>
    [AutoInject]
    public interface IEditProjectCommand
    {
        /// <summary>
        /// Calls methods to edit the existing project. Returns true if project edited.
        /// </summary>
        /// <param name="projectId">Project id to update the project.</param>
        /// <param name="request">Data to update the project.</param>
        /// <returns></returns>
        OperationResultResponse<bool> Execute(Guid projectId, JsonPatchDocument<EditProjectRequest> request);
    }
}
