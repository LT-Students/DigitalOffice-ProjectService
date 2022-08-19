using System;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public FileAccessType Access { get; set; }
  }
}
