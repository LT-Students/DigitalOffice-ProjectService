using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public record CreateImageRequest
    {
        public Guid ProjectOrTaskId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Extension { get; set; }
        public ImageType Image { get; set; }
    }
}
