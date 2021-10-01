using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser
{
    public class ProjectUserRequest
    {
        public Guid UserId { get; set; }
        public ProjectUserRoleType Role { get; set; }
    }
}
