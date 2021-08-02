using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class UserRepository : IUserRepository
    {
        public readonly IDataProvider _provider;

        private IQueryable<DbProjectUser> CreateGetPredicates(
            FindDbProjectsUserFilter filter,
            IQueryable<DbProjectUser> dbProjectUser)
        {
            if (filter.UserId.HasValue)
            {
                dbProjectUser = dbProjectUser.Where(x => x.UserId == filter.UserId);
            }

            if (filter.IncludeProject.HasValue && filter.IncludeProject.Value)
            {
                dbProjectUser = dbProjectUser.Include(x => x.Project);
            }

            return dbProjectUser;
        }

        public UserRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActive)
        {
            IQueryable<DbProjectUser> dbProjectQueryable = _provider.ProjectsUsers.AsQueryable();

            if (showNotActive)
            {
                dbProjectQueryable = dbProjectQueryable.Where(x => x.ProjectId == projectId);
            }
            else
            {
                dbProjectQueryable = dbProjectQueryable.Where(x => x.ProjectId == projectId && x.IsActive);
            }

            return dbProjectQueryable;
        }

        public void AddUsersToProject(IEnumerable<DbProjectUser> dbProjectUsers, Guid projectId)
        {
            if (dbProjectUsers == null)
            {
                throw new ArgumentNullException(nameof(dbProjectUsers));
            }

            _provider.ProjectsUsers.AddRange(dbProjectUsers);
            _provider.Save();
        }

        public IEnumerable<DbProjectUser> Find(Guid userId)
        {
            return Find(new FindDbProjectsUserFilter { UserId = userId });
        }

        public IEnumerable<DbProjectUser> Find(FindDbProjectsUserFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbProjectsUser = _provider.ProjectsUsers.AsQueryable();

            return CreateGetPredicates(filter, dbProjectsUser).ToList();
        }

        public bool AreUserProjectExist(Guid userId, Guid projectId)
        {
            return _provider.ProjectsUsers.FirstOrDefault(x => x.UserId == userId && x.ProjectId == projectId) != null;
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.ProjectsUsers.Select(x => x.UserId);

            return ids.All(x => dbIds.Contains(x));
        }

        public List<DbProjectUser> Find(List<Guid> userIds)
        {
            return _provider.ProjectsUsers.Where(u => userIds.Contains(u.UserId)).ToList();
        }
    }
}
