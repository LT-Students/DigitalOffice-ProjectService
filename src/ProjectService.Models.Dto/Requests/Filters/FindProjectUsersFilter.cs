using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindProjectUsersFilter : BaseFindFilter
  {
    [FromQuery(Name = "isActive")]
    public bool? IsActive { get; set; }

    [FromQuery(Name = "ascendingSort")]
    public bool? AscendingSort { get; set; }

    [FromQuery(Name = "includeAvatars")]
    public bool IncludeAvatars { get; set; } = false;

    [FromQuery(Name = "includePositions")]
    public bool IncludePositions { get; set; } = false;
  }
}
