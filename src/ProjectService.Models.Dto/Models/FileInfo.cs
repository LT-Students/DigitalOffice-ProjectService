using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileInfo
  {
    public Guid Id { get; set; }
    public string Extension { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int Size { get; set; }
  }
}
