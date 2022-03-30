using System;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileAccess
  {
    public Guid FileId { get; set; }
    public AccessType Access { get; set; }
  }
}
