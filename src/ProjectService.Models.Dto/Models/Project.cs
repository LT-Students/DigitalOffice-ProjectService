using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ClosedReason { get; set; }
        public bool IsActive { get; set; }
    }
}