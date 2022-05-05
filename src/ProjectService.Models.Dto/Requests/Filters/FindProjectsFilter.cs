using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindProjectsFilter : BaseFindFilter
  {
    [FromQuery(Name = "isascendingsort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name ="projectstatus")]
    public ProjectStatusType? ProjectStatus { get; set; }

    [FromQuery(Name = "nameincludesubstring")]
    public string NameIncludeSubstring { get; set; }
  }
}
