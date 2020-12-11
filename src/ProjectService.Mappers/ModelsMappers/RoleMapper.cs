using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class RoleMapper : IRoleMapper
    {
        public Role Map(DbRole value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new Role
            {
                Id = value.Id,
                Name = value.Name,
                Description = value.Description
            };
        }
    }
}
