using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Validation.User.Interfaces
{
  [AutoInject]
  public interface IProjectUsersRequestValidator : IValidator<ProjectUsersRequest>
  {
  }
}
