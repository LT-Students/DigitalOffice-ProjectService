using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record EditProjectRequest
  {
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public ProjectStatusType Status { get; set; }
  }
}
