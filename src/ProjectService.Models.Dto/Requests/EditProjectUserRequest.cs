using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class EditProjectUserRequest
    {
        public JsonPatchDocument<DbProjectUser> Patch { get; set; }
        public Guid ProjectUserId { get; set; }
    }
}
