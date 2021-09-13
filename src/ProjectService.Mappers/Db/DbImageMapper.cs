using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
    public class DbImageMapper : IDbImageMapper
    {
        public DbImage Map(CreateImageRequest request, Guid imageId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbImage
            {
                Id = Guid.NewGuid(),
                ImageId = imageId,
                Entity = request.ProjectOrTaskId
            };
        }
    }
}
