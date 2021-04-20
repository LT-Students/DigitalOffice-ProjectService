using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class ProjectsResponse
    {

        public int TotalCount { get; set; }
        public List<ProjectInfo> Projects { get; set; } = new();
        public List<string> Errors { get; set; } = new();

    }
}
