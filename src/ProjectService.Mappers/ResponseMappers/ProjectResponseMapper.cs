using LT.DigitalOffice.ProjectService.Mappers.ResponseMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponseMappers
{
    public class ProjectResponseMapper : IProjectResponseMapper
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
                IsActive = value.IsActive,
                Name = value.Name,
                ShortName = value.ShortName,
                CreatedAt = value.CreatedAt,
                ClosedAt = value.ClosedAt,
                DepartmentId = value.DepartmentId,
                ClosedReason = value.ClosedReason,
                Description = value.Description
            };
        }
    }
}
