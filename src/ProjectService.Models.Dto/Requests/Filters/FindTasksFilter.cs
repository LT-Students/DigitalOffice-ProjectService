using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class FindTasksFilter
    {
        [FromQuery(Name = "number")]
        public int? Number { get; set; }

        [FromQuery(Name = "projectid")]
        public Guid? ProjectId { get; set; }

        [FromQuery(Name="assignedto")]
        public Guid? AssignedTo { get; set; }
    }
}
