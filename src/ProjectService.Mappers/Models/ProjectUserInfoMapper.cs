using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Interfaces
{
    public class ProjectUserInfoMapper : IProjectUserInfoMapper
    {
        public ProjectUserInfo Map(
            UserData userData,
            ImageInfo image,
            PositionData userPosition,
            DepartmentData userDepartment,
            DbProjectUser dbProjectUser,
            int projectCount)
        {
            if (dbProjectUser == null)
            {
                throw new ArgumentNullException(nameof(dbProjectUser));
            }

            if (userData.Id != dbProjectUser.UserId)
            {
                throw new ArgumentException("Something went wrong while converting user data.");
            }

            return new ProjectUserInfo
            {
                Id = userData.Id,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                MiddleName = userData.MiddleName,
                Status = userData.Status,
                Rate = userData.Rate,
                ProjectCount = projectCount,
                IsActive = dbProjectUser.IsActive,
                AddedOn = dbProjectUser.AddedOn,
                RemovedOn = dbProjectUser.RemovedOn,
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
