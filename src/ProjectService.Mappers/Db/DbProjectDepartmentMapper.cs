using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectDepartmentMapper : IDbProjectDepartmentMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbProjectDepartmentMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbProjectDepartment Map(Guid projectId, Guid departmentId)
    {
      return new DbProjectDepartment
      {
        Id = Guid.NewGuid(),
        ProjectId = projectId,
        DepartmentId = departmentId,
        IsActive = true,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId()
      };
    }
  }
}
