﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;

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

    public async Task<bool> RemoveAsync(List<Guid> filesIds)
    {
      if (filesIds == null)
      {
        return false;
      }

      IEnumerable<DbProjectFile> files = _provider.ProjectsFiles
        .Where(x => filesIds.Contains(x.FileId));

      _provider.ProjectsFiles.RemoveRange(files);
      await _provider.SaveAsync();

      return true;
    }
  }
}