﻿using System;
using LT.DigitalOffice.ProjectService.Models.Dto;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for deleting user role in project
    /// </summary>
    public interface IDeleteRoleCommand
    {
        /// <summary>
        /// Call repository for deleting role in project.
        /// </summary>
        /// <param name="roleId">Id of role to be deleted.</param>
        /// <returns>Returns success of delete.</returns>
        bool Execute(Guid roleId);
    }
}
