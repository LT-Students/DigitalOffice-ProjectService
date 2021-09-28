using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbEntityImageMapper : IDbEntityImageMapper
  {
    public DbEntityImage Map(Guid imageId, Guid projectId)
    {
      return new DbEntityImage
      {
        Id = Guid.NewGuid(),
        ImageId = imageId,
        EntityId = projectId
      };
    }
  }
}
