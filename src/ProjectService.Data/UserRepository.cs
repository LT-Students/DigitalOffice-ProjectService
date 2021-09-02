using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
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

        public bool AreUserProjectExist(Guid userId, Guid projectId, bool? isManager)
        {
            if (isManager.HasValue && isManager.Value)
            {
                return _provider
                    .ProjectsUsers
                    .Any(x => x.UserId == userId && x.ProjectId == projectId && x.Role == (int)ProjectUserRoleType.Manager && x.IsActive);
            }

            return _provider
                .ProjectsUsers
                .Any(x => x.UserId == userId && x.ProjectId == projectId && x.IsActive);
        }

        public List<DbProjectUser> Find(List<Guid> userIds)
        {
            return _provider.ProjectsUsers.Where(u => userIds.Contains(u.UserId)).ToList();
        }

        public void Remove(Guid userId, Guid removedBy)
        {
            List<DbProjectUser> users = _provider.ProjectsUsers.Where(u => u.UserId == userId && u.IsActive).ToList();

            foreach (var user in users)
            {
                user.IsActive = false;
                user.ModifiedBy = removedBy;
                user.ModifiedAtUtc = DateTime.UtcNow;
            }

            _provider.Save();
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.ProjectsUsers.Where(x => x.IsActive).Select(x => x.UserId);
            return ids.All(x => dbIds.Contains(x));
        }

        public List<DbProjectUser> Find(Guid projectId, int? skipCount, int? takeCount, out int totalCount)
        {
            IQueryable<DbProjectUser> users = _provider.ProjectsUsers.Where(pu => pu.ProjectId == projectId && pu.IsActive).AsQueryable();

            totalCount = users.Count();

            if (skipCount.HasValue)
            {
                users = users.Skip(skipCount.Value);
            }

            if (takeCount.HasValue)
            {
                users = users.Take(takeCount.Value);
            }

            return users.ToList();
        }

        public List<DbProjectUser> Get(Guid projectId, int? skipCount, int? takeCount, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public List<DbProjectUser> Get(IGetProjectsUsersRequest request, out int totalCount)
        {
            IQueryable<DbProjectUser> projectUsers = _provider.ProjectsUsers.AsQueryable();

            if (request.UsersIds != null && request.UsersIds.Any())
            {
                projectUsers = projectUsers.Where(pu => request.UsersIds.Contains(pu.UserId));
            }

            if (request.ProjectsIds != null && request.ProjectsIds.Any())
            {
                projectUsers = projectUsers.Where(pu => request.ProjectsIds.Contains(pu.ProjectId));
            }

            if (!request.IncludeDisactivated)
            {
                projectUsers = projectUsers.Where(pu => pu.IsActive);
            }

            totalCount = projectUsers.Count();

            if (request.SkipCount.HasValue)
            {
                projectUsers = projectUsers.Skip(request.SkipCount.Value);
            }

            if (request.TakeCount.HasValue)
            {
                projectUsers = projectUsers.Take(request.TakeCount.Value);
            }

            return projectUsers.ToList();
        }
    }
}
