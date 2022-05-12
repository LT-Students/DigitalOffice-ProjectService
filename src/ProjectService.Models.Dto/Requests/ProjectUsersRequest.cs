using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record ProjectUsersRequest
  {
    public Guid ProjectId { get; set; }
    public List<CreateUserRequest> Users { get; set; }
  }
}
