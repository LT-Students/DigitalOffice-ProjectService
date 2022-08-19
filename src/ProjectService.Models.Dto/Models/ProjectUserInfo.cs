using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record ProjectUserInfo
  {
    public Guid UserId { get; set; }
    public ProjectUserRoleType Role { get; set; }
  }
}
