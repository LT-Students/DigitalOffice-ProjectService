﻿using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class FindProjectsResponseMapper : IFindProjectsResponseMapper
    {
        private readonly IProjectInfoMapper _mapper;
        public FindProjectsResponseMapper(
            IProjectInfoMapper mapper)
        {
            _mapper = mapper;
        }

        public ProjectsResponse Map(List<DbProject> dbProjects, int totalCount, string departmentName, List<string> errors)
        {
            if (dbProjects == null)
            {
                throw new ArgumentNullException(nameof(dbProjects));
            }

            var projectInfos = new List<ProjectInfo>();
            foreach(var dbProject in dbProjects)
            {
                projectInfos.Add(_mapper.Map(dbProject, departmentName));
            }

            return new ProjectsResponse
            {
                TotalCount = totalCount,
                Projects = projectInfos,
                Errors = errors
            };
        }
    }
}
