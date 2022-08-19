using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class FileInfoMapper : IFileInfoMapper
  {
    public FileInfo Map(FileCharacteristicsData file, FileAccessType access)
    {
      if (file is null)
      {
        return null;
      }

      return new FileInfo
      {
        Id = file.Id,
        Name = file.Name,
        Extension = file.Extension,
        Size = file.Size,
        CreatedAtUtc = file.CreatedAtUtc,
        Access = access
      };
    }
  }
}
