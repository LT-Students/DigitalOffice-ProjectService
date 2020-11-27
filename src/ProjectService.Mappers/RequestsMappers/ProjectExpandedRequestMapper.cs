using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class ProjectExpandedRequestMapper : IProjectExpandedRequestMapper
    {
        private readonly IProjectUserRequestMapper _projectUserMapper;
        public ProjectExpandedRequestMapper(IProjectUserRequestMapper projectUserMapper)
        {
            _projectUserMapper = projectUserMapper;
        }

        public DbProject Map(ProjectExpandedRequest project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var projectId = project.Project.Id ?? Guid.NewGuid();

            return new DbProject
            {
                Id = projectId,
                Name = project.Project.Name,
                ShortName = project.Project.ShortName,
                Description = project.Project.Description,
                DepartmentId = project.Project.DepartmentId,
                IsActive = project.Project.IsActive,
                Users = project.Users
                    .Select(u => _projectUserMapper.Map(u))
                    .Select(dbU => { dbU.ProjectId = projectId; return dbU; })
                    .ToList()
            };

        }
    }
}
