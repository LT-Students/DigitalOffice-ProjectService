using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels
{
    public class ProjectUser
    {
        public Guid Id { get; set; }
        public UserRoleType Role { get; set; }
    }
}
