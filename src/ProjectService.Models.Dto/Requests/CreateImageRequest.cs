using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public record CreateImageRequest
    {
        public Guid ProjectOrTaskId { get; set; }
        public List<ImageContext> Images { get; set; }
        public ImageType ImageType { get; set; }
    }
}
