﻿using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Models
{
  public record FileIdentifier
  {
    public Guid ProjectId { get; set; }
    public Guid FileId { get; set; }
  }
}
