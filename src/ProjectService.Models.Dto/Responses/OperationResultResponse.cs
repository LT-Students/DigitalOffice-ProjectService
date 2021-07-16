using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class OperationResultResponse<T>
    {
        public T Body { get; set; }
        public string Status { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
