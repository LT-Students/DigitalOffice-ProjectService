using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto
{
    public class ProjectWorker
    {
        public Guid WorkerId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsManager { get; set; }
    }
}
