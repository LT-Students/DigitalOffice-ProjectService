using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbProjectUserMapper
  {
    DbProjectUser Map(CreateUserRequest request, Guid projectId);

    List<DbProjectUser> Map(CreateProjectUsersRequest request);
  }
}
