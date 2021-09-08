using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;
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

        public List<Guid> Create(IEnumerable<DbProjectImage> images)
        {
            if (images == null)
            {
                return null;
            }

            _provider.ProjectsImages.AddRange(images);
            _provider.Save();

            return images.Select(x => x.ImageId).ToList();
        }

        public bool Remove(IEnumerable<Guid> imagesIds)
        {
            if (imagesIds == null)
            {
                return false;
            }

            IEnumerable<DbProjectImage> images = _provider.
                ProjectsImages.
                Where(x => imagesIds.
                Contains(x.ImageId));

            _provider.ProjectsImages.RemoveRange(images);
            _provider.Save();

            return true;
        }
    }
}
