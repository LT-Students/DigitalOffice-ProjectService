﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
  [AutoInject]
  public interface IProjectRepository
  {
    Task<DbProject> GetAsync(GetProjectFilter filter);

    Task<List<DbProject>> GetAsync(IGetProjectsRequest request);

    Task<DbProject> GetProjectWithUserAsync(Guid projectId, Guid userId);

    Task<(List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount)> FindAsync(FindProjectsFilter filter);

    Task CreateAsync(DbProject dbProject);

    Task<bool> EditAsync(Guid projectId, JsonPatchDocument<DbProject> request);

    Task<List<DbProject>> SearchAsync(string text);

    Task<bool> DoesExistAsync(Guid projectId);

    Task<List<Guid>> DoExistAsync(List<Guid> projectsIds);

    Task<bool> DoesNameExistAsync(string name, Guid? projectId = null);

    Task<bool> DoesShortNameExistAsync(string shortName, Guid? projectId = null);
  }
}
