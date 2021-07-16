using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class FindProjectsFilter
    {
        [FromQuery(Name = "departmentid")]
        public Guid? DepartmentId { get; set; }
    }
}
