using LT.DigitalOffice.ProjectService.Mappers.RequestMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestMappers
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
                AddedOn = DateTime.Now,
                IsActive = projectUser.User.IsActive
            };
        }
    }
}
