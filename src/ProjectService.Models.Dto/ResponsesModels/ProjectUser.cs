using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class ProjectUser
    {
        public Guid ProjectId { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
        public bool IsActive { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}
