﻿using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
    public class RolesResponse
    {
        public IEnumerable<Role> Roles { get; set; }
        public int TotalCount { get; set; }
    }
}
