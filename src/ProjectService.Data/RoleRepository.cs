using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDataProvider provider;

        public RoleRepository([FromServices] IDataProvider provider)
        {
            this.provider = provider;
        }

        public bool DeleteRole(Guid roleId)
        {
            DbRole dbRole = provider.Roles.FirstOrDefault(role => role.Id == roleId);

            if (dbRole == null)
            {
                throw new NullReferenceException("Role with this Id does not exist.");
            }

            provider.Roles.Remove(dbRole);
            provider.Save();

            DbRole dbRoleDeleted = provider.Roles.FirstOrDefault(role => role.Id == roleId);

            return dbRoleDeleted == null;
        }
    }
}
