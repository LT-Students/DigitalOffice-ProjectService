using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.ProjectUser.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.ProjectUser
{
  public class EditProjectUsersRequestValidator : AbstractValidator<EditProjectUsersRequest>, IEditProjectUsersRequestValidator
  {
    public EditProjectUsersRequestValidator()
    {
      
    }
  }
}
