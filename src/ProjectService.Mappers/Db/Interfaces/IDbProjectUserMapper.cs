﻿using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbProjectUserMapper
  {
    DbProjectUser Map(UserRequest request, Guid projectId);

    List<DbProjectUser> Map(Guid projectId, List<UserRequest> users);
  }
}
