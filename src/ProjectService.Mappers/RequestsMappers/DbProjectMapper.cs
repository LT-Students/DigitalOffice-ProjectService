using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class DbProjectMapper : IDbProjectMapper
    {
        public DbProject Map(ProjectRequest request, Guid authorId, List<Guid> users, List<Guid> imagesIds)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var projectId = Guid.NewGuid();
            string shortName = request.ShortName?.Trim();
            string description = request.Description?.Trim();
            string shortDescription = request.ShortDescription?.Trim();

            return new DbProject
            {
                Id = projectId,
                AuthorId = authorId,
                Name = request.Name,
                Status = (int)request.Status,
                ShortName = shortName == null || !shortName.Any() ? null : shortName,
                Description = description == null || !description.Any() ? null : description,
                ShortDescription = shortDescription == null || !shortDescription.Any() ? null : shortDescription,
                DepartmentId = request.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                Users = users
                    .Select(userId => new DbProjectUser
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        UserId = userId,
                        Role = (int)request.Users.FirstOrDefault(u => u.UserId == userId).Role,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    })
                    .ToList(),
                ProjectImages = imagesIds.Select(imageId => new DbProjectImage
                {
                    Id = Guid.NewGuid(),
                    ImageId = imageId,
                    ProjectId = projectId
                }).ToList()
            };

        }
    }
}
