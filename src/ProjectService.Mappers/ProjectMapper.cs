using System;
using System.Linq;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;

namespace LT.DigitalOffice.ProjectService.Mappers
{
    public class ProjectMapper : IMapper<DbProject, Project>, IMapper<NewProjectRequest, DbProject>, IMapper<EditProjectRequest, DbProject>
    {
        public Project Map(DbProject dbProject)
        {
            if (dbProject == null)
            {
                throw new ArgumentNullException(nameof(dbProject));
            }

            return new Project
            {
                Name = dbProject.Name,
                ShortName = dbProject.ShortName,
                WorkersIds = dbProject.WorkersUsersIds?.Select(x => x.WorkerUserId).ToList()
            };
        }

        public DbProject Map(NewProjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbProject
            {
                Id = Guid.NewGuid(),
                ShortName = request.ShortName,
                DepartmentId = request.DepartmentId,
                Description = request.Description,
                IsActive = request.IsActive,
                Name = request.Name,
                Deferred = false
            };
        }

        public DbProject Map(EditProjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbProject
            {
                Id = request.Id,
                ShortName = request.ShortName,
                DepartmentId = request.DepartmentId,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                Deferred = false
            };
        }
    }
}