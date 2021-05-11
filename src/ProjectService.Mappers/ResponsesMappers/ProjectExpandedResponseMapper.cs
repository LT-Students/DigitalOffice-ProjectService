using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class ProjectExpandedResponseMapper : IProjectExpandedResponseMapper
    {
        private readonly IProjectInfoMapper _projectInfoMapper;

        public ProjectExpandedResponseMapper(IProjectInfoMapper projectInfoMapper)
        {
            _projectInfoMapper = projectInfoMapper;
        }

        public ProjectExpandedResponse Map(
            DbProject dbProject,
            IEnumerable<ProjectUserInfo> users,
            IEnumerable<ProjectFileInfo> files,
            DepartmentInfo department,
            List<string> errors)
        {
            if (dbProject == null)
            {
                throw new ArgumentNullException(nameof(dbProject));
            }

            if (department?.Id != null && department.Id != dbProject.DepartmentId)
            {
                throw new ArgumentException("DepartmentId not valid.");
            }

            return new ProjectExpandedResponse
            {
                Project = _projectInfoMapper.Map(dbProject, department?.Name),
                Users = users,
                Files = files,
                Errors = errors
            };
        }
    }
}
