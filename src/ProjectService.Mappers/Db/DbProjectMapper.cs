using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectMapper : IDbProjectMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbProjectDepartmentMapper _departmentMapper;
    private readonly IDbProjectUserMapper _projectUserMapper;
    private readonly IDbProjectImageMapper _imageMapper;

    public DbProjectMapper(
      IHttpContextAccessor httpContextAccessor,
      IDbProjectDepartmentMapper departmentMapper,
      IDbProjectUserMapper projectUserMapper,
      IDbProjectImageMapper imageMapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _departmentMapper = departmentMapper;
      _projectUserMapper = projectUserMapper;
      _imageMapper = imageMapper;
    }

    public DbProject Map(CreateProjectRequest request, List<Guid> imagesIds)
    {
      if (request is null)
      {
        return null;
      }

      Guid projectId = Guid.NewGuid();

      return new DbProject
      {
        Id = projectId,
        Name = request.Name.Trim(),
        ShortName = request.ShortName.Trim(),
        Description = String.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
        ShortDescription = String.IsNullOrWhiteSpace(request.ShortDescription) ? null : request.ShortDescription,
        Customer = request.Customer,
        Status = (int)request.Status,
        StartDateUtc = request.StartDateUtc,
        EndDateUtc = request.EndDateUtc,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        Department = request.DepartmentId.HasValue ? _departmentMapper.Map(projectId, request.DepartmentId.Value) : null,
        Users = request.Users?
          .Select(pu => _projectUserMapper.Map(pu, projectId))
          .ToList(),
        Images = imagesIds?
          .Select(imageId => _imageMapper.Map(imageId, projectId))
          .ToList()
      };
    }
  }
}
