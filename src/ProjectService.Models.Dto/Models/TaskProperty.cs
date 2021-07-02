using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public class TaskProperty
    {
        public string Name { get; set; }
        public TaskPropertyType PropertyType { get; set; }
        public string Description { get; set; }
    }
}
