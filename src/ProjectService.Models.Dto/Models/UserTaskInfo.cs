using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public record UserTaskInfo
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
