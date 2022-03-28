using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectFileMapper : IDbProjectFileMapper
  {
    public DbProjectFile Map(Guid fileId, Guid projectId, AccessType accessType)
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
