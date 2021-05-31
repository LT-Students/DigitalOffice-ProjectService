using System;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
 
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
 
            return new TaskPropertyInfo()
            {
                Id = dbTaskProperty.Id,
                AuthorId = dbTaskProperty.AuthorId,
                ProjectId = dbTaskProperty.ProjectId,
                PropertyType = dbTaskProperty.PropertyType,
                Name = dbTaskProperty.Name,
                Description = dbTaskProperty.Description,
                CreatedAt = dbTaskProperty.CreatedAt,
                IsActive = dbTaskProperty.IsActive
            };
        }
    }
}