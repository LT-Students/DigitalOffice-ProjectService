using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
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
                Id = Guid.NewGuid(),
                UserId = projectUser.UserId,
                Role = (int)projectUser.Role,
                AddedOn = DateTime.Now,
                IsActive = true
            };
        }
    }
}
