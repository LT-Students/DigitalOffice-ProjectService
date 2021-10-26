﻿using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Interfaces
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public UserInfo Map(
      UserData userData,
      ImageInfo image,
      PositionData userPosition,
      DepartmentData userDepartment,
      DbProjectUser dbProjectUser,
      int projectCount)
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
        Status = userData.Status,
        Rate = userData.Rate,
        ProjectCount = projectCount,
        IsActive = dbProjectUser.IsActive,
        CreatedAtUtc = dbProjectUser.CreatedAtUtc,
        ModifiedAtUtc = dbProjectUser.ModifiedAtUtc,
        AvatarImage = image,
        Role = (ProjectUserRoleType)dbProjectUser.Role,
        Department = userDepartment == null
          ? null
          : new()
          {
            Id = userDepartment.Id,
            Name = userDepartment.Name
          },
        Position = userPosition == null
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