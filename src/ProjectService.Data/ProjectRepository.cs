﻿using LinqKit;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDataProvider provider;

        public ProjectRepository([FromServices] IDataProvider provider)
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

        public Guid CreateNewProject(DbProject newProject)
        {
            provider.Projects.Add(newProject);
            provider.Save();

            return newProject.Id;
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

        public void DisableWorkersInProject(ProjectRequest request)
        {
            DbProject dbProject = provider.Projects
                .FirstOrDefault(p => p.Id == request.Project.Id);

            if (dbProject == null)
            {
                throw new NullReferenceException("Project with this Id does not exist.");
            }

            foreach (ProjectUser projectUser in request.Users)
            {
                DbProjectUser dbProjectWorker = dbProject.Users?.FirstOrDefault(w => w.UserId == projectUser.User.Id);

                if (dbProjectWorker == null)
                {
                    throw new NullReferenceException("Worker with this Id does not exist.");
                }

                dbProjectWorker.IsActive = false;
            }

            provider.Projects.Update(dbProject);
            provider.Save();
        }

        public IEnumerable<DbProject> GetProjects(bool showNotActive)
        {
            var predicate = PredicateBuilder.New<DbProject>(p => p.IsActive);
            if (showNotActive)
            {
                predicate.Or(p => !p.IsActive);
            }

            return provider.Projects.Where(predicate).ToList();
        }

        public DbRole GetRole(Guid roleId)
        {
            var result = provider.Roles.FirstOrDefault(r => r.Id == roleId);
            if (result == null)
            {
                throw new NotFoundException($"Role with id: '{roleId}' was not found.");
            }

            return result;
        }
    }
}