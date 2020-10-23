using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class RoleRepository: IRoleRepository
    {
        private readonly IDataProvider provider;

        public RoleRepository([FromServices] IDataProvider provider)
        {
            this.provider = provider;
        }

        public Guid CreateRole(DbRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(DbRole));
            }
            provider.Roles.Add(role);
            provider.SaveModelsChanges();

            return role.Id;
        }
    }
}
