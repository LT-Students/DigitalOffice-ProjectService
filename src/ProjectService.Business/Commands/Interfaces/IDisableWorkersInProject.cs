﻿using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// </summary>
    public interface IDisableWorkersInProjectCommand
    {
        /// <summary>
        /// Call repository for disabling workers from project.
        /// </summary>
        /// <param name="request"></param>
        void Execute(ProjectRequest request);
    }
}
