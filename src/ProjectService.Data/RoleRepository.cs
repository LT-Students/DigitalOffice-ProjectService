using LinqKit;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public DbRole GetRole(Guid roleId)
        {
            var result = provider.Roles.FirstOrDefault(r => r.Id == roleId);
            if (result == null)
            {
                throw new NotFoundException($"Role with id: '{roleId}' was not found.");
            }

            return result;
        }

        public Guid CreateRole(DbRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(DbRole));
            }

            if ((provider.Roles.FirstOrDefault(r => r.Id == role.Id)) != null)
            {
                throw new ArgumentException($"Role with this id is already exist");
            }

            provider.Roles.Add(role);
            provider.Save();

            return role.Id;
        }
    }
}