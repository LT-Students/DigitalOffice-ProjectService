using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class UserRepository : IUserRepository
    {
        public readonly IDataProvider _provider;

        public UserRepository([FromServices] IDataProvider provider)
        {
            _provider = provider;
        }

        public void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUser)
        {
            if (dbProjectUser == null)
            {
                throw new ArgumentNullException("List project users is null");
            }

            _provider.ProjectsUsers.AddRange(dbProjectUser);
        }
    }
}
