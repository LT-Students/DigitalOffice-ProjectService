using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models.User
{
    public class UserRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
