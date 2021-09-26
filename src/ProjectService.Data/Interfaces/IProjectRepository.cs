using System;
using System.Collections.Generic;
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
    DbProject Get(GetProjectFilter filter);

    IEnumerable<DbProject> Get(Guid departmentId);

    List<DbProject> Find(List<Guid> projectIds);

    Guid Create(DbProject dbProject);

    bool Edit(Guid projectId, JsonPatchDocument<DbProject> request);

    void DisableWorkersInProject(Guid projectId, IEnumerable<Guid> userIds);

    List<DbProject> Find(FindProjectsFilter filter, out int totalCount);

    List<DbProject> Search(string text);

    List<DbProject> Get(IGetProjectsRequest request, out int totalCount);

    bool IsExist(Guid id);

    bool DoesProjectNameExist(string name);
  }
}
