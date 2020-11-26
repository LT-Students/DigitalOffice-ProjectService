using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.RequestModels
{
    public class ProjectUserRequest
    {
        public Guid? ProjectId { get; set; }
        public Guid RoleId { get; set; }
        public UserRequest User { get; set; }
    }
}
