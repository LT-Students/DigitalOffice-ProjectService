using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class TaskPropertyRepository : ITaskPropertyRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskPropertyRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Create(IEnumerable<DbTaskProperty> dbTaskProperties)
    {
      _provider.TaskProperties.AddRange(dbTaskProperties);
      _provider.Save();
    }

    public bool AreExist(params Guid[] ids)
    {
      IQueryable<Guid> dbIds = _provider.TaskProperties.Select(x => x.Id);

      return ids.All(x => dbIds.Contains(x));
    }

    public bool AreExistForProject(Guid projectId, params string[] propertyNames)
    {
      IQueryable<string> dbPropertyNames = _provider.TaskProperties
        .Where(tp => tp.ProjectId == projectId || tp.ProjectId == null)
        .Select(x => x.Name);

      return propertyNames.Any(x => dbPropertyNames.Contains(x));
    }

    public bool Edit(DbTaskProperty taskProperty, JsonPatchDocument<DbTaskProperty> taskPatch)
    {
      taskPatch.ApplyTo(taskProperty);
      taskProperty.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      taskProperty.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();

      return true;
    }

    public DbTaskProperty Get(Guid propertyId)
    {
      return _provider.TaskProperties.FirstOrDefault(x => x.Id == propertyId);
    }

    public IEnumerable<DbTaskProperty> Find(FindTaskPropertiesFilter filter, out int totalCount)
    {
      IQueryable<DbTaskProperty> dbTaskProperties = _provider.TaskProperties.AsQueryable();

      if (filter.ProjectId.HasValue)
      {
        dbTaskProperties = dbTaskProperties.Where(tp => tp.ProjectId == filter.ProjectId.Value || tp.ProjectId == null);
      }

      if (filter.AuthorId.HasValue)
      {
        dbTaskProperties = dbTaskProperties.Where(tp => tp.CreatedBy == filter.AuthorId.Value);
      }

      if (!string.IsNullOrEmpty(filter.Name))
      {
        dbTaskProperties = dbTaskProperties.Where(tp => tp.Name.Contains(filter.Name));
      }

      if (filter.Type.HasValue)
      {
        dbTaskProperties = dbTaskProperties.Where(tp => tp.PropertyType == (int)filter.Type.Value);
      }

      totalCount = dbTaskProperties.Count();

      return dbTaskProperties.Skip(filter.SkipCount).Take(filter.TakeCount).ToList();
    }

    public bool AreExist(Guid id, TaskPropertyType type)
    {
      return _provider.TaskProperties.Any(tp => tp.Id == id && tp.IsActive && tp.PropertyType == (int)type);
    }
  }
}
