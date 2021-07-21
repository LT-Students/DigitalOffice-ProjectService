﻿using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
    public record ProjectTaskInfo
    {
        public Guid Id { get; set; }
        public string ShortName { get; set; }
    }
}
