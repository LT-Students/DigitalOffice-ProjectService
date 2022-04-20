using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateUserRequest
  {
    public Guid UserId { get; set; }
    public ProjectUserRoleType Role { get; set; }
  }
}
