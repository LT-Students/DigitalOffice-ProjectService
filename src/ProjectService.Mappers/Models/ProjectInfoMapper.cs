using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class ProjectInfoMapper : IProjectInfoMapper
    {
        public ProjectInfo Map(DbProject value, DepartmentInfo department)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new ProjectInfo
            {
                Id = value.Id,
                Name = value.Name,
                CreatedBy = value.CreatedBy,
                Status = (ProjectStatusType)value.Status,
                CreatedAtUtc = value.CreatedAtUtc,
                ShortName = value.ShortName,
                Description = value.Description,
                ShortDescription = value.ShortDescription,
                Department = department
            };
        }
    }
}

