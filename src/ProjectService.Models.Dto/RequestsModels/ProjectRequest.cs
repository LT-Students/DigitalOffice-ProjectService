using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels
{
    public class ProjectRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ClosedReason { get; set; }
        public string Description { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }
}
