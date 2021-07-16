using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
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
                Status = ((ProjectStatusType)value.Status).ToString(),
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

