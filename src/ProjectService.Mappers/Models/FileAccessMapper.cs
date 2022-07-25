using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
  public class FileAccessMapper : IFileAccessMapper
  {
    public FileAccess Map(DbProjectFile file)
    {
      if (file is null)
      {
        return null;
      }

      return new FileAccess
      {
        FileId = file.FileId,
        Access = (FileAccessType)file.Access
      };
    }
  }
}
