using System;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record EditProjectUsersRequest
  {
    public Guid ProjectId { get; set; }
    public List<Guid> UsersIds { get; set; }
    public List<ProjectUserRoleType> RoleTypes { get; set; }
  }
}
