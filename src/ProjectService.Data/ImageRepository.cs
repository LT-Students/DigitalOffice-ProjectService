using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;

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

    public async Task<List<Guid>> CreateAsync(List<DbProjectImage> images)
    {
      if (images == null)
      {
        return null;
      }

      _provider.Images.AddRange(images);
      await _provider.SaveAsync();

      return images.Select(x => x.ImageId).ToList();
    }

    public async Task<bool> RemoveAsync(List<Guid> imagesIds)
    {
      if (imagesIds == null)
      {
        return false;
      }

      IEnumerable<DbProjectImage> images = _provider.Images
        .Where(x => imagesIds.Contains(x.ImageId));

      _provider.Images.RemoveRange(images);
      await _provider.SaveAsync();

      return true;
    }
  }
}
