using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class ProjectInfoMapper : IProjectInfoMapper
    {
        public ProjectInfo Map(DbProject value, string departmentName)
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
                Status = (ProjectStatusType)value.Status,
                CreatedAt = value.CreatedAt,
                ShortName = value.ShortName,
                Description = value.Description,
                ShortDescription = value.ShortDescription,
                Department = new DepartmentInfo
                {
                    Id = value.DepartmentId,
                    Name = departmentName
                }
            };
        }
    }
}