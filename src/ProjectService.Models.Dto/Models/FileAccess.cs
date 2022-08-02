using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileAccess
  {
    public Guid FileId { get; set; }
    public FileAccessType Access { get; set; }
  }
}
