using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
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
                ClosedReason = value.ClosedReason,
                IsActive = value.IsActive
            };
        }
    }
}