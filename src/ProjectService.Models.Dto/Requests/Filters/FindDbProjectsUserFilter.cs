﻿using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
    public class FindDbProjectsUserFilter
    {
        public Guid? UserId { get; set; }
        public bool? IncludeProject { get; set; }
    }
}
