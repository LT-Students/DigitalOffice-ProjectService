using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.ProjectUser.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.ProjectUser
{
  public class ProjectUsersRequestValidator : AbstractValidator<ProjectUsersRequest>, IProjectUsersRequestValidator
  {
    public ProjectUsersRequestValidator()
    {
    }
  }
}
