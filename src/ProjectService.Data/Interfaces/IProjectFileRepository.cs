using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.File;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  [AutoInject]
  public interface IProjectFileRepository
  {
    Task<List<Guid>> CreateAsync(List<DbProjectFile> files);

    Task<bool> RemoveAsync(List<Guid> filesIds);

    Task<(List<DbProjectFile>, int filesCount)> FindAsync(FindProjectFilesFilter filter, FileAccessType access = FileAccessType.Manager);

    Task<List<DbProjectFile>> GetAsync(List<Guid> filesIds);

    Task<bool> EditAsync(Guid fileId, FileAccessType access);
  }
}
