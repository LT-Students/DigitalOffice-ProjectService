using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
  public class PatchDbTaskPropertyMapper : IPatchDbTaskPropertyMapper
  {
    public JsonPatchDocument<DbTaskProperty> Map(JsonPatchDocument<TaskProperty> request)
    {
      if (request == null)
      {
        return null;
      }

      JsonPatchDocument<DbTaskProperty> result = new JsonPatchDocument<DbTaskProperty>();

      foreach (Operation<TaskProperty> item in request.Operations)
      {
        if (item.path.EndsWith(nameof(TaskProperty.PropertyType), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbTaskProperty>(item.op, item.path, item.from, (int)Enum.Parse(typeof(TaskPropertyType), item.value.ToString())));
          continue;
        }

        result.Operations.Add(new Operation<DbTaskProperty>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}
