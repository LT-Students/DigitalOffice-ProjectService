using LT.DigitalOffice.ProjectService.Mappers.ModelMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelMappers
{
    public class ProjectMapper : IProjectMapper
    {
        public Project Map(DbProject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new Project
            {
                Id = value.Id,
                Name = value.Name,
                ShortName = value.ShortName,
                Description = value.Description,
                ClosedAt = value.ClosedAt,
                CreatedAt = value.CreatedAt,
                ClosedReason = value.ClosedReason ?? null,
                IsActive = value.IsActive
            };
        }
    }
}