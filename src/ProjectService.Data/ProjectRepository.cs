using LT.DigitalOffice.Kernel.Exceptions.Models;
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

        public IEnumerable<DbProject> Get(Guid departmentId)
        {
            return _provider.Projects.Where(p => p.DepartmentId == departmentId);
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

        public List<DbProject> FindProjects(FindProjectsFilter filter, int skipCount, int takeCount, out int totalCount)
        {
            if (skipCount < 0 )
            {
                throw new BadRequestException("Skip count can't be less than 0.");
            }

            if (takeCount <= 0)
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

            dbProjects = dbProjects.Skip(skipCount).Take(takeCount);

            return dbProjects.ToList();
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
    }
}