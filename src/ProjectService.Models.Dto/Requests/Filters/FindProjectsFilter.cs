using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters
{
    public class FindProjectsFilter
    {
        [FromQuery(Name = "name")]
        public string Name { get; set; }

        [FromQuery(Name = "shortname")]
        public string ShortName { get; set; }

        [FromQuery(Name = "departmentname")]
        public string DepartmentName { get; set; }
    }
}
