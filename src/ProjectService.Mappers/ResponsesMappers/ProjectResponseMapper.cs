using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class ProjectResponseMapper : IProjectResponseMapper
    {
        public ProjectInfo Map(DbProject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new ProjectInfo
            {
                Id = value.Id,
                Name = value.Name,
                AuthorId = value.AuthorId,
                ShortName = value.ShortName,
                CreatedAt = value.CreatedAt,
                DepartmentId = value.DepartmentId,
                Status = (ProjectStatusType)value.Status,
                Description = value.Description,
                ShortDescription = value.ShortDescription
            };
        }
    }
}
