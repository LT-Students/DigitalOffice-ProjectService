﻿using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class ProjectExpandedRequest
    {
        public ProjectRequest Project { get; set; }
        public IEnumerable<ProjectUserRequest> Users { get; set; }
    }
}
