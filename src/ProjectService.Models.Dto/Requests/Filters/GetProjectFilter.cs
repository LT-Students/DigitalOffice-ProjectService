using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class GetProjectFilter
    {
        [FromQuery(Name = "projectId")]
        public Guid ProjectId { get; set; }

        [FromQuery(Name = "includeusers")]
        public bool? IncludeUsers { get; set; }

        [FromQuery(Name = "shownotactiveusers")]
        public bool? ShowNotActiveUsers { get; set; }

        [FromQuery(Name = "includefiles")]
        public bool? IncludeFiles { get; set; }
    }
}
