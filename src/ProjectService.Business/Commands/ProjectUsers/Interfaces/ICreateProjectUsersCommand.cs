using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces
{
  /// <summary>
  /// Represents interface for a command in command pattern.
  /// Provides method for adding a new users to project.
  /// </summary>
  [AutoInject]
  public interface ICreateProjectUsersCommand
  {
    /// <summary>
    /// Added new users id to specific project.
    /// </summary>
    /// <param name="request">List of users for a specific project.</param>
    Task<OperationResultResponse<bool>> ExecuteAsync(ProjectUsersRequest request);
  }
}
