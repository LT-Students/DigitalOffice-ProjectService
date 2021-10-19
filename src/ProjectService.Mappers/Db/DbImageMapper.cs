using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbImageMapper : IDbImageMapper
  {
    public DbEntityImage Map(CreateImageRequest request, Guid imageId)
    {
      if (request == null)
      {
        return null;
      }

      return new DbEntityImage
      {
        Id = Guid.NewGuid(),
        ImageId = imageId,
        EntityId = request.ProjectId
      };
    }
  }
}
