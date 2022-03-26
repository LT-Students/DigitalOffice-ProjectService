using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class DbImageMapper : IDbImageMapper
  {
    public DbProjectImage Map(CreateImagesRequest request, Guid imageId)
    {
      if (request == null)
      {
        return null;
      }

      return new DbProjectImage
      {
        Id = Guid.NewGuid(),
        ImageId = imageId,
        ProjectId = request.ProjectId
      };
    }
  }
}
