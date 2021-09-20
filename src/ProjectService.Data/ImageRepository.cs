using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class ImageRepository : IImageRepository
    {
        private readonly IDataProvider _provider;

        public ImageRepository(
            IDataProvider provider)
        {
            _provider = provider;
        }

        public List<Guid> Create(IEnumerable<DbEntityImage> images)
        {
            if (images == null)
            {
                return null;
            }

            _provider.Images.AddRange(images);
            _provider.Save();

            return images.Select(x => x.ImageId).ToList();
        }

        public bool Remove(IEnumerable<Guid> imagesIds)
        {
            if (imagesIds == null)
            {
                return false;
            }

            IEnumerable<DbEntityImage> images = _provider.Images
                .Where(x => imagesIds.Contains(x.ImageId));

            _provider.Images.RemoveRange(images);
            _provider.Save();

            return true;
        }
    }
}
