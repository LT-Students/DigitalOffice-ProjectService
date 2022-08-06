using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IFileInfoMapper
  {
    FileInfo Map(FileCharacteristicsData file, FileAccessType access);
  }
}
