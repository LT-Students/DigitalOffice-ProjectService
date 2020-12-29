using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class AddUsersToProjectRequest
    {
        public Guid ProjectId { get; set; }
        public IEnumerable<ProjectUserRequest> Users { get; set; }
    }
}