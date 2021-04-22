using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser
{
    public class ProjectUserRequest
    {
        public Guid Id { get; set; }
        public UserRoleType Role { get; set; }
    }
}
