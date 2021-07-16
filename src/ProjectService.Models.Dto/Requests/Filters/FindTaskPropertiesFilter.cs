using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class FindTaskPropertiesFilter
    {
        [FromQuery(Name="name")]
        public string Name { get; set; }

        [FromQuery(Name="projectid")]
        public Guid? ProjectId { get; set; }

        [FromQuery(Name = "authorid")]
        public Guid? AuthorId { get; set; }
    }
}
