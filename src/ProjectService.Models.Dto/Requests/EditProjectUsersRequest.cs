using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record EditProjectUsersRequest
  {
    public List<Guid> UsersIds { get; set; }
    public List<int> RoleTypes { get; set; }
  }
}
