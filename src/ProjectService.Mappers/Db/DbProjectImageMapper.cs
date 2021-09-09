using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
    public class DbProjectImageMapper : IDbProjectImageMapper
    {
        public DbProjectImage Map(CreateImageRequest request, Guid imageId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbProjectImage
            {
                Id = Guid.NewGuid(),
                ImageId = imageId,
                ProjectId = request.ProjectOrTaskId
            };
        }
    }
}
