using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    class FindDbTasksFilter
    {
        public int Number { get; set; }
        public Guid ProjectId { get; set; }
        public Guid Assign { get; set; }
    }
}
