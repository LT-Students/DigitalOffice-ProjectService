using System;
using System.Linq;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;

namespace LT.DigitalOffice.ProjectService.Mappers
{
    public class RoleMapper : IMapper<DbRole, Role>, IMapper<CreateRoleRequest, DbRole>
    {
        public Role Map(DbRole dbRole)
        {
            if (dbRole == null)
            {
                throw new ArgumentNullException(nameof(dbRole));
            }

            return new Role
            {
                Name = dbRole.Name,
                Description = dbRole.Description
            };
        }

        public DbRole Map(CreateRoleRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbRole
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };
        }
    }
}