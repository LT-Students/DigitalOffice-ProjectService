﻿using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectMapper : IDbProjectMapper
  {
    private readonly IDbEntityImageMapper _dbEntityImageMapper;

    public DbProjectMapper(IDbEntityImageMapper dbEntityImageMapper)
    {
      _dbEntityImageMapper = dbEntityImageMapper;
    }

    public DbProject Map(CreateProjectRequest request, Guid authorId, List<Guid> users, List<Guid> departmentIds, List<Guid> imagesIds)
    {
      if (request == null)
      {
        return null;
      }

      Guid projectId = Guid.NewGuid();
      string shortName = request.ShortName?.Trim();
      string description = request.Description?.Trim();
      string shortDescription = request.ShortDescription?.Trim();

      return new DbProject
      {
        Id = projectId,
        Name = request.Name,
        Status = (int)request.Status,
        ShortName = shortName == null || !shortName.Any() ? null : shortName,
        Description = description == null || !description.Any() ? null : description,
        ShortDescription = shortDescription == null || !shortDescription.Any() ? null : shortDescription,
        DepartmentId = request.DepartmentId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = authorId,
        Users = users
          .Select(userId => new DbProjectUser
          {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            UserId = userId,
            Role = (int)request.Users.FirstOrDefault(u => u.UserId == userId).Role,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = authorId,
            IsActive = true
          })
          .ToList(),
        Images = imagesIds.Select(imageId => _dbEntityImageMapper.Map(imageId, projectId)).ToList()
      };
    }
  }
}
