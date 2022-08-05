using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data
{
  public class ProjectDepartmentRepository : IProjectDepartmentRepository
  {

    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectDepartmentRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> CreateAsync(DbProjectDepartment request)
    {
      if (request is null)
      {
        return null;
      }

      _provider.ProjectsDepartments.Add(request);
      await _provider.SaveAsync();

      return request.Id;
    }

    public async Task<bool> EditAsync(Guid projectId, Guid? departmentId)
    {
      DbProjectDepartment dbProjectDepartment = await _provider.ProjectsDepartments
        .FirstOrDefaultAsync(dp => dp.ProjectId == projectId);

      if (dbProjectDepartment is null)
      {
        return false;
      }

      dbProjectDepartment.DepartmentId = departmentId.HasValue ? departmentId.Value : dbProjectDepartment.DepartmentId;
      dbProjectDepartment.IsActive = departmentId.HasValue ? true : false;
      dbProjectDepartment.CreatedBy = _httpContextAccessor.HttpContext.GetUserId();

      await _provider.SaveAsync();

      return true;
    }
  }
}
