using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class EditProjectRequest
    {
        public JsonPatchDocument<DbProject> Patch { get; set; }
        public Guid ProjectId { get; set; }
    }
}
