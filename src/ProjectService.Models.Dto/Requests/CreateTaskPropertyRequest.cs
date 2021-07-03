using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class CreateTaskPropertyRequest
    {
        public Guid ProjectId { get; set; }
        public IEnumerable<TaskProperty> TaskProperties { get; set; }
    }
}
