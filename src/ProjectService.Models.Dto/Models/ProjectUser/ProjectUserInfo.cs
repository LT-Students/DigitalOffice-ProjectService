using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.User;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser
{
    public class ProjectUserInfo
    {
        public Guid ProjectId { get; set; }
        public UserInfo User { get; set; }
        public UserRoleType Role { get; set; }
    }
}
