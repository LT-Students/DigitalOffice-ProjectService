﻿using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDataProvider _provider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectRepository(
            IDataProvider provider,
            IHttpContextAccessor httpContextAccessor)
        {
            _provider = provider;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbProject Get(GetProjectFilter filter)
        {
            IQueryable<DbProject> dbProjectQueryable = _provider.Projects.AsQueryable();

            if (filter.IncludeUsers.HasValue && filter.IncludeUsers.Value)
            {
                if (filter.ShowNotActiveUsers.HasValue && filter.ShowNotActiveUsers.Value)
                {
                    dbProjectQueryable = dbProjectQueryable.Include(x => x.Users);
                }
                else
                {
                    dbProjectQueryable = dbProjectQueryable.Include(x => x.Users.Where(x => x.IsActive));
                }
            }

            if (filter.IncludeFiles.HasValue && filter.IncludeFiles.Value)
            {
                dbProjectQueryable = dbProjectQueryable.Include(x => x.Files);
            }

            if (filter.IncludeImages.HasValue && filter.IncludeImages.Value)
            {
                dbProjectQueryable = dbProjectQueryable.Include(x => x.ProjectsImages);
            }

            var dbProject = dbProjectQueryable.FirstOrDefault(x => x.Id == filter.ProjectId);

            if (dbProject == null)
            {
                throw new NotFoundException($"Project with id: '{filter.ProjectId}' was not found.");
            }

            return dbProject;
        }

        public IEnumerable<DbProject> Get(Guid departmentId)
        {
            return _provider.Projects.Where(p => p.DepartmentId == departmentId);
        }

        public Guid Create(DbProject dbProject)
        {
            _provider.Projects.Add(dbProject);
            _provider.Save();

            return dbProject.Id;
        }

        public bool Edit(DbProject dbProject, JsonPatchDocument<DbProject> request)
        {
            request.ApplyTo(dbProject);
            dbProject.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
            dbProject.ModifiedAtUtc = DateTime.UtcNow;
            _provider.Save();

            return true;
        }

        public void DisableWorkersInProject(Guid projectId, IEnumerable<Guid> userIds)
        {
            DbProject dbProject = _provider.Projects
                .FirstOrDefault(p => p.Id == projectId);

            if (dbProject == null)
            {
                throw new NotFoundException($"Project with Id {projectId} does not exist.");
            }

            foreach (var userId in userIds)
            {
                DbProjectUser dbProjectUser = dbProject.Users?.FirstOrDefault(w => w.UserId == userId);

                if (dbProjectUser == null)
                {
                    throw new NotFoundException($"Worker with Id {userId} does not exist.");
                }

                dbProjectUser.IsActive = false;
            }

            _provider.Projects.Update(dbProject);
            _provider.Save();
        }

        public List<DbProject> Find(FindProjectsFilter filter, int skipCount, int takeCount, out int totalCount)
        {
            if (skipCount < 0 )
            {
                throw new BadRequestException("Skip count can't be less than 0.");
            }

            if (takeCount < 1)
            {
                throw new BadRequestException("Take count can't be equal or less than 0.");
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbProjects = _provider.Projects
                .AsQueryable();

            if (filter.DepartmentId.HasValue)
            {
                dbProjects = dbProjects.Where(p => p.DepartmentId == filter.DepartmentId.Value);
                totalCount = dbProjects.Count(p => p.DepartmentId == filter.DepartmentId.Value);
            }
            else
            {
                totalCount = dbProjects.Count();
            }

            return dbProjects.Skip(skipCount).Take(takeCount).ToList();
        }

        public List<DbProject> Search(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            return _provider.Projects.Where(p => p.Name.Contains(text) || p.ShortName.Contains(text)).ToList();
        }

        public bool IsExist(Guid id)
        {
            return _provider.Projects.FirstOrDefault(x => x.Id == id) != null;
        }

        public bool IsProjectNameExist(string name)
        {
            return _provider.Projects.Any(p => p.Name.Contains(name));
        }

        public List<DbProject> Find(List<Guid> projectIds)
        {
            return _provider.Projects.Where(p => projectIds.Contains(p.Id)).ToList();
        }

        public Dictionary<Guid, List<Guid>> GetProjectsUsers()
        {
            List<Tuple<Guid, Guid>> projectsUsers = _provider
                .ProjectsUsers
                .Where(pu => pu.IsActive)
                .Select(pu => new Tuple<Guid, Guid>(pu.ProjectId, pu.UserId))
                .ToList();

            Dictionary<Guid, List<Guid>> response = new();
            foreach(var pair in projectsUsers)
            {
                if (!response.ContainsKey(pair.Item1))
                {
                    response.Add(pair.Item1, new() { pair.Item2 });
                }
                else
                {
                    response[pair.Item1].Add(pair.Item2);
                }
            }

            return response;
        }
    }
}