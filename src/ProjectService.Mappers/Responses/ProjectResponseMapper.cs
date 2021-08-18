using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses
{
    public class ProjectResponseMapper : IProjectResponseMapper
    {
        private readonly IProjectInfoMapper _projectInfoMapper;

        public ProjectResponseMapper(IProjectInfoMapper projectInfoMapper)
        {
            _projectInfoMapper = projectInfoMapper;
        }

        public ProjectResponse Map(
            DbProject dbProject,
            IEnumerable<ProjectUserInfo> users,
            IEnumerable<ProjectFileInfo> files,
            DepartmentInfo department)
        {
            if (dbProject == null)
            {
                throw new ArgumentNullException(nameof(dbProject));
            }

            if (department?.Id != null && department.Id != dbProject.DepartmentId)
            {
                throw new ArgumentException("DepartmentId not valid.");
            }

            return new ProjectResponse
            {
                Project = _projectInfoMapper.Map(dbProject, department?.Name),
                Users = users,
                Files = files,
            };
        }
    }
}
