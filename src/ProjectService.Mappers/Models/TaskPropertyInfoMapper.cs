﻿using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class TaskPropertyInfoMapper : ITaskPropertyInfoMapper
    {
        public TaskPropertyInfo Map(DbTaskProperty dbTaskProperty)
        {
            if (dbTaskProperty == null)
            {
                throw new ArgumentNullException(nameof(dbTaskProperty));
            }

            return new TaskPropertyInfo
            {
                Id = dbTaskProperty.Id,
                ProjectId = dbTaskProperty.ProjectId,
                CreatedBy = dbTaskProperty.CreatedBy,
                Name = dbTaskProperty.Name,
                CreatedAtUtc = dbTaskProperty.CreatedAtUtc,
                Description = dbTaskProperty.Description,
                IsActive = dbTaskProperty.IsActive,
                PropertyType = (TaskPropertyType)dbTaskProperty.PropertyType
            };
        }
    }
}
