using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters
{
    public class FindDbProjectsFilter
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public List<Guid> DepartmentIds { get; set; }
    }
}
