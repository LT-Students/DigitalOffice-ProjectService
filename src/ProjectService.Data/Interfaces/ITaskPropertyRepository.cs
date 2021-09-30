using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  [AutoInject]
  public interface ITaskPropertyRepository
  {
    void Create(IEnumerable<DbTaskProperty> dbTaskProperties);

    bool AreExist(params Guid[] ids);

    bool AreExist(Guid id, TaskPropertyType type);

    bool AreExistForProject(Guid projectId, params string[] propertyNames);

    DbTaskProperty Get(Guid propertyId);

    IEnumerable<DbTaskProperty> Find(FindTaskPropertiesFilter filter, out int totalCount);

    bool Edit(DbTaskProperty taskProperty, JsonPatchDocument<DbTaskProperty> taskPatch);
  }
}
