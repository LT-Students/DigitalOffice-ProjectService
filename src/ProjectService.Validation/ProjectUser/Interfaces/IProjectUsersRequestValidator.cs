using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Validation.ProjectUser.Interfaces
{
  [AutoInject]
  public interface IProjectUsersRequestValidator : IValidator<ProjectUsersRequest>
  {
  }
}
