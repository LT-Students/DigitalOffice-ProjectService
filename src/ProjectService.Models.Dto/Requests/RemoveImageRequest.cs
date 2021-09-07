﻿using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public record RemoveImageRequest
    {
        public Guid ProjectOrTaskId { get; set; }
        public ImageType ImageType { get; set; }
    }
}
