﻿using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectMapper : IDbProjectMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbProjectUserMapper _projectUserMapper;
    private readonly IDbEntityImageMapper _dbEntityImageMapper;

    public DbProjectMapper(
      IHttpContextAccessor httpContextAccessor,
      IDbProjectUserMapper projectUserMapper,
      IDbEntityImageMapper dbEntityImageMapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _projectUserMapper = projectUserMapper;
      _dbEntityImageMapper = dbEntityImageMapper;
    }

    public DbProject Map(CreateProjectRequest request, List<Guid> imagesIds)
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
        Name = request.Name.Trim(),
        Status = (int)request.Status,
        ShortName = shortName == null || !shortName.Any() ? null : shortName,
        Description = description == null || !description.Any() ? null : description,
        ShortDescription = shortDescription == null || !shortDescription.Any() ? null : shortDescription,
        DepartmentId = request.DepartmentId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        Users = request.Users?
          .Select(pu => _projectUserMapper.Map(pu, projectId))
          .ToList(),
        Images = imagesIds?
          .Select(imageId => _dbEntityImageMapper.Map(imageId, projectId))
          .ToList()
      };
    }
  }
}
