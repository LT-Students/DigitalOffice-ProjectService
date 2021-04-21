﻿using LT.DigitalOffice.Kernel.Attributes;
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
    public interface ICreateNewProjectCommand
    {
        /// <summary>
        /// Adds a new project. Returns id of the added project.
        /// </summary>
        /// <param name="request">Project data.</param>
        /// <returns>Id of the added project.</returns>
        OperationResultResponse<ProjectInfo> Execute(ProjectRequest request);
    }
}
