using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileInfo
  {
    public string Name { get; set; }
    public string Content { get; set; }
    public string Extension { get; set; }
    public AccessType Access { get; set; }
  }
}
