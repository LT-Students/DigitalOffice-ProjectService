using System;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
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
        result.Operations.Add(new Operation<DbTaskProperty>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}
