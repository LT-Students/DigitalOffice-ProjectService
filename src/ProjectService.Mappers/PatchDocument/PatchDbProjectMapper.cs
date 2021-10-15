using LT.DigitalOffice.ProjectService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
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
        dbRequest.Operations.Add(new Operation<DbProject>(item.op, item.path, item.from, item.value));
      }

      return dbRequest;
    }
  }
}
