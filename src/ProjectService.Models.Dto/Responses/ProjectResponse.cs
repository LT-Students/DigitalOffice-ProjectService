using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsActive { get; set; }
    }
}
