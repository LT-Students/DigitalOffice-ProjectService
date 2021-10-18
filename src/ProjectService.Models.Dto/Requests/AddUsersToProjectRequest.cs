using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record AddUsersToProjectRequest
  {
    public Guid ProjectId { get; set; }
    public List<AddUserRequest> Users { get; set; }
  }
}
