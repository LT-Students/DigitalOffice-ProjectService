using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class UserRepository : IUserRepository
    {
        public readonly IDataProvider _provider;

        public UserRepository([FromServices] IDataProvider provider)
        {
            _provider = provider;
        }

        public void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUsers , Guid projectId)
        {
            if (dbProjectUsers == null)
            {
                throw new ArgumentNullException("List project users is null");
            }

            if (!_provider.Projects.Any(p => p.Id == projectId))
            {
                throw new BadRequestException("Project with this Id does not exist.");
            }

            _provider.ProjectsUsers.AddRange(dbProjectUsers);
        }


    }
}
