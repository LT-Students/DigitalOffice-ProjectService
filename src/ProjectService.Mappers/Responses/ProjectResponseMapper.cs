﻿using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

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
      IEnumerable<UserInfo> users,
      IEnumerable<Guid> files,
      IEnumerable<ImageInfo> images,
      DepartmentInfo department)
    {
      if (dbProject == null)
      {
        return null;
      }

      return new ProjectResponse
      {
        Project = _projectInfoMapper.Map(dbProject, department),
        Users = users,
        Files = files,
        Images = images
      };
    }
  }
}
