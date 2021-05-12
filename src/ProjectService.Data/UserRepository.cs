using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class UserRepository : IUserRepository
    {
        public readonly IDataProvider _provider;

        public UserRepository(IDataProvider provider)
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

        public List<DbProjectUser> Find(Guid userId)
        {
            return _provider.ProjectsUsers.Include(u => u.Project).Where(x => x.UserId == userId).ToList();
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.ProjectsUsers.Select(x => x.Id);

            return ids.All(x => dbIds.Contains(x));
        }
    }
}
