using System;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class FileDataMapper : IFileDataMapper
  {
    public FileData Map(FileContent file)
    {
      if (file is null)
      {
        return null;
      }

      return new FileData(
        Guid.NewGuid(),
        file.Name,
        file.Content,
        file.Extension);
    }
  }
}
