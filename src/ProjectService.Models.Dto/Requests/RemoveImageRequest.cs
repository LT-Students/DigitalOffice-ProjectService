using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class RemoveImageRequest
    {
        public Guid Id { get; set; }
        public ImageType Image { get; set; }
    }
}
