using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.Interfaces
{
    public class ProjectUserInfoMapper : IProjectUserInfoMapper
    {
        public ProjectUserInfo Map(UserData userData, DbProjectUser dbProjectUser)
        {
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
                IsActive = dbProjectUser.IsActive,
                AddedOn = dbProjectUser.AddedOn,
                RemovedOn = dbProjectUser.RemovedOn,
                Role = (UserRoleType)dbProjectUser.Role
            };
        }
    }
}
