using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models
{
    public class Project
    {
        public string Name { get; set; }
        public List<Guid> WorkersIds { get; set; }
    }
}