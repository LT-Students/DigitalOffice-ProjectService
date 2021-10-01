using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
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
                ProjectId = projectId,
                Name = request.Name,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = authorId,
                Description = string.IsNullOrEmpty(request.Description?.Trim()) ? null : request.Description.Trim(),
                PropertyType = (int)request.PropertyType,
            };
        }
    }
}
