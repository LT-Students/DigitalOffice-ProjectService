using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class DbTaskPropertyMapper : IDbTaskPropertyMapper
    {
        public DbTaskProperty Map(TaskProperty request, Guid authorId, Guid projectId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbTaskProperty
            {
                Id = Guid.NewGuid(),
                AuthorId = authorId,
                ProjectId = projectId,
                Name = request.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Description = request.Description,
                PropertyType = (int)request.PropertyType,
            };
        }
    }
}
