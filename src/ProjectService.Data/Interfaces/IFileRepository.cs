using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  [AutoInject]
  public interface IFileRepository
  {
    Task<List<Guid>> CreateAsync(List<DbProjectFile> files);

    Task<bool> RemoveAsync(List<Guid> filesIds);

    Task<List<DbProjectFile>> GetAsync(Guid projectId, FileAccessType access = FileAccessType.Manager);

    Task<List<DbProjectFile>> GetAsync(List<Guid> filesIds);
  }
}
