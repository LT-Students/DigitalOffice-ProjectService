using System;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Mappers.PatchDocument
{
  public class PatchDbProjectMapper : IPatchDbProjectMapper
  {
    public JsonPatchDocument<DbProject> Map(JsonPatchDocument<EditProjectRequest> request)
    {
      if (request == null)
      {
        return null;
      }

      JsonPatchDocument<DbProject> dbRequest = new();

      foreach (var item in request.Operations)
      {
        if (item.path[1..].Equals(nameof(EditProjectRequest.Status), StringComparison.OrdinalIgnoreCase))
        {
          ProjectStatusType status = Enum.Parse<ProjectStatusType>(item.value?.ToString());
          if (status == ProjectStatusType.Active)
          {
            dbRequest.Operations.Add(new Operation<DbProject>("replace", "/EndDateUtc", null, null));
          }

          dbRequest.Operations.Add(new Operation<DbProject>(item.op, item.path, item.from, (int)status));
          continue;
        }

        dbRequest.Operations.Add(new Operation<DbProject>(item.op, item.path, item.from, item.value));
      }

      return dbRequest;
    }
  }
}
