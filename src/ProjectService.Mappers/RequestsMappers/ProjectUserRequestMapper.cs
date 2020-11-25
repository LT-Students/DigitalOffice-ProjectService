using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class ProjectUserRequestMapper : IProjectUserRequestMapper
    {
        public DbProjectUser Map(ProjectUserRequest projectUser)
        {
            if (projectUser == null)
            {
                throw new ArgumentNullException(nameof(projectUser));
            }

            return new DbProjectUser
            {
                UserId = projectUser.User.Id,
                RoleId = projectUser.RoleId,
                IsActive = projectUser.User.IsActive
            };
        }
    }
}
