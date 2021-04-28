using LinqKit;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDataProvider provider;

        public ProjectRepository(IDataProvider provider)
        {
            this.provider = provider;
        }

        public DbProject GetProject(GetProjectFilter filter)
        {
            IQueryable<DbProject> dbProjectQueryable = provider.Projects.AsQueryable();

            if (filter.IncludeUsers.HasValue && filter.IncludeUsers.Value)
            {
                dbProjectQueryable = dbProjectQueryable.Include(x => x.Users);
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
            provider.Projects.Add(newProject);
            provider.Save();
        }

        public Guid EditProjectById(DbProject dbProject)
        {
            var projectToEdit = provider.Projects
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == dbProject.Id);

            if (projectToEdit == null)
            {
                throw new NullReferenceException("Project with this Id does not exist");
            }

            provider.Projects.Update(dbProject);
            provider.Save();

            return dbProject.Id;
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

        public IEnumerable<DbProject> GetProjects(bool showNotActive)
        {
            return provider.Projects.ToList();
        }
    }
}