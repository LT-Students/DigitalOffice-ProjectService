using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbEntityImageMapper
  {
    DbEntityImage Map(Guid imageId, Guid projectId);
  }
}
