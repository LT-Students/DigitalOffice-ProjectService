using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class ProjectMapper : IProjectMapper
    {
        public DbProject Map(NewProjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbProject
            {
                Id = Guid.NewGuid(),
                ShortName = request.ShortName,
                DepartmentId = request.DepartmentId,
                Description = request.Description,
                IsActive = request.IsActive,
                Name = request.Name
            };
        }

        public DbProject Map(EditProjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbProject
            {
                Id = request.Id,
                ShortName = request.ShortName,
                DepartmentId = request.DepartmentId,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };
        }

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
                ClosedReason = value.ClosedReason.HasValue ? ((ProjectClosedReason)value.ClosedReason).ToString() : null,
                IsActive = value.IsActive
            };
        }
    }
}