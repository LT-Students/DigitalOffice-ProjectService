using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class FileRepository : IFileRepository
  {
    private readonly IDataProvider _provider;

    public FileRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<List<Guid>> CreateAsync(List<DbProjectFile> files)
    {
      if (files == null)
      {
        return null;
      }

      _provider.ProjectsFiles.AddRange(files);
      await _provider.SaveAsync();

      return files.Select(x => x.FileId).ToList();
    }

    public Task<List<DbProjectFile>> GetAsync(Guid projectId, FileAccessType access = FileAccessType.Manager)
    {
      return _provider.ProjectsFiles.Where(x => x.ProjectId == projectId && x.Access >= (int)access).ToListAsync();
    }

    public Task<List<DbProjectFile>> GetAsync(List<Guid> filesIds)
    {
      return _provider.ProjectsFiles.Where(x => filesIds.Contains(x.FileId)).ToListAsync();
    }

    public async Task<bool> RemoveAsync(List<Guid> filesIds)
    {
      if (filesIds == null)
      {
        return false;
      }

      _provider.ProjectsFiles.RemoveRange(
        _provider.ProjectsFiles
        .Where(x => filesIds.Contains(x.FileId)));

      await _provider.SaveAsync();

      return true;
    }
  }
}
