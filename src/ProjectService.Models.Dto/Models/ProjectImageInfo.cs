﻿using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public record ProjectImageInfo
    {
        public Guid ProjectId { get; set; }
        public Guid ParentId { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
    }
}
