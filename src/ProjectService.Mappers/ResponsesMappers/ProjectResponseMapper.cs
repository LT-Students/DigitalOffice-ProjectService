using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class ProjectResponseMapper : IProjectResponseMapper
    {
        public ProjectResponse Map(DbProject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new ProjectResponse
            {
                Id = value.Id,
                IsActive = value.IsActive,
                Name = value.Name,
                ShortName = value.ShortName
            };
        }
    }
}
