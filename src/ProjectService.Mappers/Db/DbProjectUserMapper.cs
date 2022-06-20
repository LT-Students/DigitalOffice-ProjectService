using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectUserMapper : IDbProjectUserMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbProjectUserMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbProjectUser Map(UserRequest request, Guid projectId)
    {
      if (request == null)
      {
        return null;
      }

      return new DbProjectUser
      {
        Id = Guid.NewGuid(),
        ProjectId = projectId,
        UserId = request.UserId,
        Role = (int)request.Role,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }

    public List<DbProjectUser> Map(Guid projectId, List<UserRequest> users)
    {
      if (users is null)
      {
        return null;
      }

      return users.Select(u =>
        new DbProjectUser
        {
          Id = Guid.NewGuid(),
          ProjectId = projectId,
          UserId = u.UserId,
          Role = (int)u.Role,
          CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
          IsActive = true
        }).ToList();
    }
  }
}
