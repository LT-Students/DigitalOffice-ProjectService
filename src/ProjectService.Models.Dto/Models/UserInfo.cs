using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public class UserInfo
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    public ProjectUserRoleType Role { get; set; }
    public ImageInfo AvatarImage { get; set; }
    public PositionInfo Position { get; set; }
  }
}
