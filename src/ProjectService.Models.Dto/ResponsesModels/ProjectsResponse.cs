using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class ProjectsResponse
    {
        public int TotalCount { get; set; }
        public List<ProjectInfo> Projects { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
