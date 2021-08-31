using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
    public class DbProjectUserMapper : IDbProjectUserMapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbProjectUserMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
                IsActive = true
            };
        }
    }
}
