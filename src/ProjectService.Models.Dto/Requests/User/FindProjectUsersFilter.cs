using System;
using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.User
{
  public record FindProjectUsersFilter : BaseFindFilter
  {
    [FromQuery(Name = "isActive")]
    public bool? IsActive { get; set; }

    [FromQuery(Name = "isAscendingSort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "fullNameIncludeSubstring")]
    public string FullNameIncludeSubstring { get; set; }

    [FromQuery(Name = "includeAvatars")]
    public bool IncludeAvatars { get; set; } = false;

    [FromQuery(Name = "includePositions")]
    public bool IncludePositions { get; set; } = false;

    [FromQuery(Name = "positionId")]
    public Guid? PositionId { get; set; } = null;
  }
}
