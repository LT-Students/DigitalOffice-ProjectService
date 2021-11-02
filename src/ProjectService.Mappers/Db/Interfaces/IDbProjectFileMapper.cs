using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbProjectFileMapper
  {
    DbProjectFile Map(Guid fileId, Guid projectId);
  }
}
