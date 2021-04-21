using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class ProjectInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ShortDescription { get; set; }
        public DepartmentInfo DepartmentInfo { get; set; }
    }
}
