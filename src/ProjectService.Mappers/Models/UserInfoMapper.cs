using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Interfaces
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public UserInfo Map(
      DbProjectUser dbProjectUser,
      UserData userData,
      PositionData userPosition)
    {
      if (dbProjectUser == null)
      {
        return null;
      }

      if (userData.Id != dbProjectUser.UserId)
      {
        return null;
      }

      return new UserInfo
      {
        Id = userData.Id,
        FirstName = userData.FirstName,
        LastName = userData.LastName,
        MiddleName = userData.MiddleName,
        IsActive = dbProjectUser.IsActive,
        ImageId = userData.ImageId,
        Role = (ProjectUserRoleType)dbProjectUser.Role,
        Position = userPosition is null
          ? null
          : new()
          {
            Id = userPosition.Id,
            Name = userPosition.Name
          }
      };
    }
  }
}
