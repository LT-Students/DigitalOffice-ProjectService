﻿using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
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

        private IQueryable<DbProject> CreateFindPredicates(
            FindDbProjectsFilter filter,
            IQueryable<DbProject> dbProjects)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                dbProjects = dbProjects.Where(u => u.Name.ToUpper().Contains(filter.Name.ToUpper()));
            }

            if (!string.IsNullOrEmpty(filter.ShortName))
            {
                dbProjects = dbProjects.Where(u => u.ShortName.ToUpper().Contains(filter.ShortName.ToUpper()));
            }

            if (filter.IdNameDepartments != null)
            {
                dbProjects = dbProjects.Where(u => filter.IdNameDepartments.Keys.Contains(u.DepartmentId));
            }

            return dbProjects;
        }

        public ProjectRepository(IDataProvider provider)
        {
            this._provider = provider;
        }

        public DbProject GetProject(GetProjectFilter filter)
        {
            IQueryable<DbProject> dbProjectQueryable = _provider.Projects.AsQueryable();

            if (filter.IncludeUsers.HasValue && filter.IncludeUsers.Value)
            {
                if (filter.ShowNotActiveUsers.HasValue && !filter.ShowNotActiveUsers.Value)
                {
                    dbProjectQueryable = dbProjectQueryable.Include(x => x.Users.Where(x => x.IsActive));
                }
                else
                {
                    dbProjectQueryable = dbProjectQueryable.Include(x => x.Users);
                }
            }

            if (filter.IncludeFiles.HasValue && filter.IncludeFiles.Value)
            {
                dbProjectQueryable = dbProjectQueryable.Include(x => x.Files);
            }

            var dbProject = dbProjectQueryable.FirstOrDefault(x => x.Id == filter.ProjectId);

            if (dbProject == null)
            {
                throw new NotFoundException($"Project with id: '{filter.ProjectId}' was not found.");
            }

            return dbProject;
        }

        public void CreateNewProject(DbProject newProject)
        {
            _provider.Projects.Add(newProject);
            _provider.Save();
        }

        public bool Edit(DbProject dbProject, JsonPatchDocument<DbProject> request)
        {
            request.ApplyTo(dbProject);
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

        public List<DbProject> FindProjects(FindDbProjectsFilter filter, int skipCount, int takeCount, out int totalCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbProjects = _provider.Projects
                .AsSingleQuery()
                .AsQueryable();

            var projects = CreateFindPredicates(filter, dbProjects).ToList();
            totalCount = projects.Count;

            return projects.Skip(skipCount * takeCount).Take(takeCount).ToList();
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
    }
}