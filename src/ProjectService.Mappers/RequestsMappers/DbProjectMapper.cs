using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class DbProjectMapper : IDbProjectMapper
    {
        public DbProject Map(ProjectRequest request, Guid authorId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var projectId = Guid.NewGuid();

            return new DbProject
            {
                Id = projectId,
                AuthorId = authorId,
                Name = request.Name,
                Status = (int)request.Status,
                ShortName = request.ShortName,
                Description = request.Description,
                ShortDescription = request.ShortDescription,
                DepartmentId = request.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                Users = request.Users?
                    .Select(user => new DbProjectUser
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        UserId = user.UserId,
                        Role = (int)user.Role,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    })
                    .ToList()
            };

        }
    }
}
