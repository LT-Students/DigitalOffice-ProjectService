using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models.User
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
