using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class FindTasksFilter
    {
        [FromQuery(Name = "number")]
        public int? Number { get; set; }

        [FromQuery(Name = "projectId")]
        public Guid? ProjectId { get; set; }

        [FromQuery(Name="assignTo")]
        public Guid? Assign { get; set; }
    }
}
