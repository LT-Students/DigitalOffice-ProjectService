using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public DbProject GetProjectInfoById(Guid projectId)
        {
            var dbProject = provider.Projects.FirstOrDefault(project => project.Id == projectId);

            if (dbProject == null)
            {
                throw new Exception("Project with this id was not found.");
            }

            return dbProject;
        }

        public Guid CreateNewProject(DbProject newProject)
        {
            provider.Projects.Add(newProject);
            provider.SaveModelsChanges();

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
            provider.SaveModelsChanges();

            return dbProject.Id;
        }

        public void DisableWorkersInProject(WorkersIdsInProjectRequest request)
        {
            DbProject dbProject = provider.Projects
                .FirstOrDefault(p => p.Id == request.ProjectId);

            if (dbProject == null)
            {
                throw new NullReferenceException("Project with this Id does not exist.");
            }

            foreach (Guid workerId in request.WorkersIds)
            {
                DbProjectWorkerUser dbProjectWorker = dbProject.WorkersUsersIds?
                    .FirstOrDefault(w => w.WorkerUserId == workerId);

                if (dbProjectWorker == null)
                {
                    throw new NullReferenceException("Worker with this Id does not exist.");
                }

                dbProjectWorker.IsActive = false;
            }

            provider.Projects.Update(dbProject);
            provider.SaveModelsChanges();
        }
    }
}