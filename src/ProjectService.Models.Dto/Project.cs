﻿using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto
{
    public class Project
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public List<Guid> WorkersIds { get; set; }
    }
}