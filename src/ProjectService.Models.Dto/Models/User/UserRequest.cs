using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels
{
    public class UserRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
