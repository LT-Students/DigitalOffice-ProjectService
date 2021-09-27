using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public class AddUsersToProjectRequest
  {
    public Guid ProjectId { get; set; }
    public List<ProjectUserRequest> Users { get; set; }
  }
}
