using LinqKit;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActive)
        {
            var predicate = PredicateBuilder.New<DbProjectUser>(u => u.ProjectId == projectId && u.IsActive);

            if (showNotActive)
            {
                predicate.Or(u => !u.IsActive);
            }

            return _provider.ProjectsUsers.Include(u => u.Role).Where(predicate).ToList();
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

        public IEnumerable<DbProjectUser> Find(Guid userId)
        {
            return _provider.ProjectsUsers.Where(x => x.UserId == userId);
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.ProjectsUsers.Select(x => x.Id);

            return ids.All(x => dbIds.Contains(x));
        }
    }
}
