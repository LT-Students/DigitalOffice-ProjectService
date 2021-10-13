using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces
{
  /// <summary>
  /// Represents interface for a command in command pattern.
  /// </summary>
  [AutoInject]
  public interface IDisableWorkersInProjectCommand
  {
    /// <summary>
    /// Call repository for disabling workers from project.
    /// </summary>
    /// <param name="projectId">Project id.</param>
    /// <param name="userIds">User ids.</param>
    System.Threading.Tasks.Task Execute(Guid projectId, IEnumerable<Guid> userIds);
  }
}
