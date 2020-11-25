using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
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
