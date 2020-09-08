using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto
{
    public class WorkersIdsInProjectRequest
    {
        public Guid ProjectId { get; set; }
        public IEnumerable<Guid> WorkersIds { get; set; }
    }
}
