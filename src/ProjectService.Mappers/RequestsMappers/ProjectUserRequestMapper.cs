using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class ProjectUserRequestMapper : IProjectUserRequestMapper
    {
        public DbProjectUser Map(ProjectUser projectUser)
        {
            if (projectUser == null)
            {
                throw new ArgumentNullException(nameof(projectUser));
            }

            return new DbProjectUser
            {
                UserId = projectUser.User.Id,
                RoleId = projectUser.Role.Id,
                AddedOn = projectUser.User.AddedOn,
                RemovedOn = projectUser.User.RemovedOn,
                IsActive = projectUser.User.IsActive
            };
        }
    }
}
