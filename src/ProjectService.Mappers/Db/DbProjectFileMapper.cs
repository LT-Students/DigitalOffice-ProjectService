using System;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectFileMapper : IDbProjectFileMapper
  {
    public DbProjectFile Map(Guid fileId, Guid projectId, FileAccessType accessType)
    {
      return new DbProjectFile
      {
        Id = Guid.NewGuid(),
        FileId = fileId,
        ProjectId = projectId,
        Access = (int)accessType
      };
    }
  }
}
