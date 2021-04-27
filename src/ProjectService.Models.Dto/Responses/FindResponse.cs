using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class FindResponse<T>
    {
        public int TotalCount { get; set; }
        public List<T> Body { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}