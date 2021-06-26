using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class TaskPropertyInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TaskPropertyType PropertyType { get; set; }
    }
}
