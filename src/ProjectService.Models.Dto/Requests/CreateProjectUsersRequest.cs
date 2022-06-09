using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateProjectUsersRequest
  {
    public Guid ProjectId { get; set; }

    [Required]
    public List<UserRequest> Users { get; set; }
  }
}
