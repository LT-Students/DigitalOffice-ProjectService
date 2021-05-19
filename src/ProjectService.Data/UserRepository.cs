﻿using LT.DigitalOffice.Kernel.Exceptions.Models;
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
            GetDbProjectsUserFilter filter,
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

        public IEnumerable<DbProjectUser> Get(Guid userId)
        {
            return Get(new GetDbProjectsUserFilter { UserId = userId });
        }

        public IEnumerable<DbProjectUser> Get(GetDbProjectsUserFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbProjectsUser = _provider.ProjectsUsers
                .AsSingleQuery()
                .AsQueryable();

            return CreateGetPredicates(filter, dbProjectsUser).ToList();
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.ProjectsUsers.Select(x => x.Id);

            return ids.All(x => dbIds.Contains(x));
        }
    }
}
