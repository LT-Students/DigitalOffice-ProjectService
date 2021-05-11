using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class GetProjectFilter
    {
        [FromQuery(Name = "projectid")]
        public Guid ProjectId { get; set; }

        [FromQuery(Name = "includeusers")]
        public bool? IncludeUsers { get; set; }

        [FromQuery(Name = "shownotactiveusers")]
        public bool? ShowNotActiveUsers { get; set; }

        [FromQuery(Name = "includefiles")]
        public bool? IncludeFiles { get; set; }
    }
}
