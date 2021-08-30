using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class CreateImageRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Extension { get; set; }
        public ImageType Image { get; set; }
    }
}
