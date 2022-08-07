using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Mappers.PatchDocument.Interfaces
{
  [AutoInject]
  public interface IPatchDbProjectMapper
  {
    JsonPatchDocument<DbProject> Map(JsonPatchDocument<EditProjectRequest> request);
  }
}
