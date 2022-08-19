using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.User
{
  public record UserRequest
  {
    public Guid UserId { get; set; }
    public ProjectUserRoleType Role { get; set; }
  }
}
