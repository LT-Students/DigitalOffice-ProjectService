using System;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class FileDataMapper : IFileDataMapper
  {
    public FileData Map(FileInfo file, List<FileAccess> accesses)
    {
      if (file is null)
      {
        return null;
      }

      Guid fileId = Guid.NewGuid();
      accesses.Add(new FileAccess
      {
        FileId = fileId,
        Access = file.Access
      });

      return new FileData(
        fileId,
        file.Name,
        file.Content,
        file.Extension);
    }
  }
}
