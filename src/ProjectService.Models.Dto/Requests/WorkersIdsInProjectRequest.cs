using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class WorkersIdsInProjectRequest
    {
        public Guid ProjectId { get; set; }
        public IEnumerable<Guid> WorkersIds { get; set; }
    }
}
