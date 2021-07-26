using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class UserTaskInfo
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
