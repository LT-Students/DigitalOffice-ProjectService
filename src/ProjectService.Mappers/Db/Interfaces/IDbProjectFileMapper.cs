using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbProjectFileMapper
  {
    DbProjectFile Map(Guid fileId, Guid projectId, AccessType accessType);
  }
}
