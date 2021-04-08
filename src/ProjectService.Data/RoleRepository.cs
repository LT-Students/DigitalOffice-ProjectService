using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDataProvider provider;

        public RoleRepository(IDataProvider provider)
        {
            this.provider = provider;
        }

        public bool DisableRole(Guid roleId)
        {
            DbRole dbRole = provider.Roles.FirstOrDefault(role => role.Id == roleId);

            if (dbRole == null)
            {
                throw new NullReferenceException("Role with this Id does not exist.");
            }

            dbRole.IsActive = false;

            provider.Roles.Update(dbRole);
            provider.Save();

            DbRole dbRoleRemoved = provider.Roles.FirstOrDefault(role => role.Id == roleId);

            return dbRoleRemoved.IsActive == false;
        }
    }
}
