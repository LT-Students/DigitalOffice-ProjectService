using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser
{
    public class ProjectUserInfo
    {
        public Guid Id { get; set; }
        public Guid? ImageId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public double Rate { get; set; }
        public int ProjectCount { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
        public bool IsActive { get; set; }
        public UserRoleType Role { get; set; }
        public DepartmentInfo Department { get; set; }
        public PositionInfo Position { get; set; }
    }
}
