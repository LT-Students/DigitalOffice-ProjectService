﻿using System.ComponentModel.DataAnnotations;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileInfo
  {
    [Required]
    public string Name { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public string Extension { get; set; }
    public AccessType Access { get; set; }
  }
}
