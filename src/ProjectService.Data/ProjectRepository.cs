using LinqKit;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDataProvider provider;

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
            this.provider = provider;
        }

        public DbProject GetProject(Guid projectId)
        {
            var dbProject = provider.Projects.FirstOrDefault(p => p.Id == projectId);

            if (dbProject == null)
            {
                throw new NotFoundException($"Project with id: '{projectId}' was not found.");
            }

            return dbProject;
        }

        public IEnumerable<DbProjectUser> GetProjectUsers(Guid projectId, bool showNotActive)
        {
            var predicate = PredicateBuilder.New<DbProjectUser>(u => u.ProjectId == projectId && u.IsActive);

            if (showNotActive)
            {
                predicate.Or(u => !u.IsActive);
            }

            return provider.ProjectsUsers.Include(u => u.Role).Where(predicate).ToList();
        }

        public void CreateNewProject(DbProject newProject)
        {
            provider.Projects.Add(newProject);
            provider.Save();
        }

        public bool EditProject(Guid projectId, JsonPatchDocument<DbProject> request)
        {
            var projectToEdit = GetProject(projectId);

            request.ApplyTo(projectToEdit);
            provider.Save();

            return true;
        }

        public void DisableWorkersInProject(Guid projectId, IEnumerable<Guid> userIds)
        {
            DbProject dbProject = provider.Projects
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

            provider.Projects.Update(dbProject);
            provider.Save();
        }

        public List<DbProject> FindProjects(FindDbProjectsFilter filter, int skipCount, int takeCount, out int totalCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbProjects = provider.Projects
                .AsSingleQuery()
                .AsQueryable();

            var projects = CreateFindPredicates(filter, dbProjects).ToList();
            totalCount = projects.Count;

            return projects.Skip(skipCount * takeCount).Take(takeCount).ToList();
        }
    }
}