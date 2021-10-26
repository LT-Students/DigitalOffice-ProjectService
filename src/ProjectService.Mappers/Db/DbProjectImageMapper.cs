using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbProjectImageMapper : IDbProjectImageMapper
  {
    public DbProjectImage Map(Guid imageId, Guid projectId)
    {
      return new DbProjectImage
      {
        Id = Guid.NewGuid(),
        ImageId = imageId,
        EntityId = projectId
      };
    }
  }
}
