using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  [AutoInject]
  public interface IProjectRepository
  {
    Task<DbProject> GetAsync(GetProjectFilter filter);

    Task<(List<DbProject>, int totalCount)> GetAsync(IGetProjectsRequest request);

    Task<(List<DbProject>, int totalCount)> FindAsync(FindProjectsFilter filter);

    Task<Guid?> CreateAsync(DbProject dbProject);

    Task<bool> EditAsync(Guid projectId, JsonPatchDocument<DbProject> request);

    Task<List<DbProject>> SearchAsync(string text);

    Task<bool> DoesExistAsync(Guid projectId);

    Task<List<Guid>> DoExistAsync(List<Guid> projectsIds);

    Task<bool> DoesProjectNameExistAsync(string name);
  }
}
