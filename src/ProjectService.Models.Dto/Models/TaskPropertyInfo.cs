using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public record TaskPropertyInfo
    {
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public bool IsActive { get; set; }
        public TaskPropertyType PropertyType { get; set; }
    }
}
