using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class DbTaskPropertyMapper : IDbTaskPropertyMapper
    {
        public DbTaskProperty Map(TaskProperty request, Guid userId)
        {
            return new DbTaskProperty
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IsActive = true,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow,
                Description = request.Description,
                PropertyType = (int)request.PropertyType,
            };
        }
    }
}
