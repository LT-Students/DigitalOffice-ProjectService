using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class UserTaskInfoMapper : IUserTaskInfoMapper
    {
        public UserTaskInfo Map(UserData user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserTaskInfo
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName
            };
        }
    }
}
