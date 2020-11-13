using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class CreateRoleRequestMapper : ICreateRoleRequestMapper
    {
        public CreateRoleRequest Map(DbRole value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new CreateRoleRequest
            {
                Name = value.Name,
                Description = value.Description
            };
        }

        public DbRole Map(CreateRoleRequest value)
        {
            return new DbRole()
            {
                Name = value.Name,
                Description = value.Description
            };
        }
    }
}
