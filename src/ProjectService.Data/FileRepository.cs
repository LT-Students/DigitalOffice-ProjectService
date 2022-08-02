using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
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

    public async Task<(List<DbProjectFile>, int filesCount)> FindAsync(FindProjectFilesFilter filter, FileAccessType access = FileAccessType.Manager)
    {
      if (filter is null)
      {
        return (null, 0);
      }

      IQueryable<DbProjectFile> dbFilesQuery = _provider.ProjectsFiles
        .AsQueryable();

      return (
        await dbFilesQuery.Where(file => file.ProjectId == filter.ProjectId && file.Access >= (int)access)
          .Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(),
        await dbFilesQuery.CountAsync());
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
