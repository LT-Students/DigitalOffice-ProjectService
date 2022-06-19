using LT.DigitalOffice.Models.Broker.Models.Image;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class ImageInfoMapper : IImageInfoMapper
  {
    public ImageInfo Map(ImageData image)
    {
      if (image == null)
      {
        return null;
      }

      return new()
      {
        Id = image.ImageId,
        ParentId = image.ParentId,
        Name = image.Name,
        Content = image.Content,
        Extension = image.Extension
      };
    }
  }
}
