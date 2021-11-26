﻿using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateProjectUsersRequest
  {
    public Guid ProjectId { get; set; }
    public List<CreateUserRequest> Users { get; set; }
  }
}
