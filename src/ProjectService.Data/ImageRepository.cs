using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class ImageRepository : IImageRepository
    {
        private readonly IDataProvider _provider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageRepository(
            IDataProvider provider,
            IHttpContextAccessor httpContextAccessor)
        {
            _provider = provider;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Create(IEnumerable<DbProjectImage> images)
        {
            _provider.ProjectsImages.AddRange(images);
            _provider.Save();

            return true;
        }

        public bool Delete(List<Guid> imageIds)
        {
            throw new NotImplementedException();
        }
    }
}
