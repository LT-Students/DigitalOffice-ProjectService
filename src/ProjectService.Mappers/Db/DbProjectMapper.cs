using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectMapper : IDbProjectMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbProjectUserMapper _projectUserMapper;
    private readonly IDbProjectImageMapper _dbEntityImageMapper;
    private readonly IDbProjectFileMapper _dbProjectFileMapper;

    public DbProjectMapper(
      IHttpContextAccessor httpContextAccessor,
      IDbProjectUserMapper projectUserMapper,
      IDbProjectImageMapper dbEntityImageMapper,
      IDbProjectFileMapper dbProjectFileMapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _projectUserMapper = projectUserMapper;
      _dbEntityImageMapper = dbEntityImageMapper;
      _dbProjectFileMapper = dbProjectFileMapper;
    }

    public DbProject Map(CreateProjectRequest request, List<Guid> imagesIds, List<FileAccess> accesses)
    {
      if (request is null)
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
        ShortName = shortName is null || !shortName.Any() ? null : shortName,
        Description = description is null || !description.Any() ? null : description,
        ShortDescription = shortDescription is null || !shortDescription.Any() ? null : shortDescription,
        Customer = request.Customer,
        StartDateUtc = request.StartDateUtc == default ? DateTime.UtcNow : request.StartDateUtc.Value,
        EndDateUtc = request.EndDateUtc,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        Users = request.Users?
          .Select(pu => _projectUserMapper.Map(pu, projectId))
          .ToList(),
        Images = imagesIds?
          .Select(imageId => _dbEntityImageMapper.Map(imageId, projectId))
          .ToList(),
        Files = accesses?
          .Select(x => _dbProjectFileMapper.Map(x.FileId, projectId, x.Access))
          .ToList()
      };
    }
  }
}
