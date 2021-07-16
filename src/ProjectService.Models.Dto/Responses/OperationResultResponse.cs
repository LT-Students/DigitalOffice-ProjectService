using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class OperationResultResponse<T>
    {
        public T Body { get; set; }
        public OperationResultStatusType Status { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
