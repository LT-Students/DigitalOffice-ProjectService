using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters
{
  public record FindProjectsFilter : BaseFindFilter
  {
    public bool? IsAscendingSort { get; set; }
    public ProjectStatusType? ProjectStatus { get; set; }
    public string NameIncludeSubstring { get; set; }
  }
}
